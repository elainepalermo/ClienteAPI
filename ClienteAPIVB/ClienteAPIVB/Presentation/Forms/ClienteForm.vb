Imports System.Windows.Forms
Imports System.Drawing

Public Class Clienteform
    Inherits Form

    Private ReadOnly _clienteservice As IClienteService
    Private clientes As List(Of Cliente)
    Private clienteSelecionadoId As Integer = 0


    Private dgvclientes As DataGridView

    Private txtfiltronome As TextBox
    Private txtfiltroemail As TextBox
    Private txtfiltrocpf As TextBox
    Private btnfiltrar As Button

    Private txtnome As TextBox
    Private txtemail As TextBox
    Private txtcpf As TextBox
    Private txtrg As TextBox

    Private btnsalvar As Button
    Private btnlimpar As Button
    Private btnexcluir As Button
    Private btncontatos As Button
    Private btnenderecos As Button
    Private linkapiurl As LinkLabel

    Public Sub New(clienteservice As IClienteService)
        _clienteservice = clienteservice
        InicializarComponentes()
        CarregarClientes()
    End Sub

    Public Sub New()
        Me.New(New ClienteService(New JsonClienteRepository()))
    End Sub

    Private Sub InicializarComponentes()
        Me.Text = "Cadastro de Clientes"
        Me.Size = New Size(900, 600)
        Me.StartPosition = FormStartPosition.CenterScreen


        Dim lblfiltronome = New Label With {.Text = "Nome:", .Left = 10, .Top = 15, .Width = 40}
        txtfiltronome = New TextBox With {.Left = 55, .Top = 10, .Width = 150}
        Dim lblfiltroemail = New Label With {.Text = "Email:", .Left = 220, .Top = 15, .Width = 40}
        txtfiltroemail = New TextBox With {.Left = 265, .Top = 10, .Width = 150}
        Dim lblfiltrocpf = New Label With {.Text = "CPF:", .Left = 430, .Top = 15, .Width = 30}
        txtfiltrocpf = New TextBox With {.Left = 465, .Top = 10, .Width = 120}
        btnfiltrar = New Button With {.Text = "Filtrar", .Left = 600, .Top = 9, .Width = 80}
        AddHandler btnfiltrar.Click, AddressOf BtnFiltrar_click


        dgvclientes = New DataGridView With {
            .Left = 10,
            .Top = 45,
            .Width = 860,
            .Height = 250,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        }
        AddHandler dgvclientes.CellClick, AddressOf DgvClientes_cellclick


        Dim lblnome = New Label With {.Text = "Nome:", .Left = 10, .Top = 310}
        txtnome = New TextBox With {.Left = 110, .Top = 305, .Width = 300}

        Dim lblemail = New Label With {.Text = "Email:", .Left = 420, .Top = 310}
        txtemail = New TextBox With {.Left = 520, .Top = 305, .Width = 300}

        Dim lblcpf = New Label With {.Text = "CPF:", .Left = 10, .Top = 350}
        txtcpf = New TextBox With {.Left = 110, .Top = 345, .Width = 150}

        Dim lblrg = New Label With {.Text = "RG:", .Left = 300, .Top = 350}
        txtrg = New TextBox With {.Left = 400, .Top = 345, .Width = 150}

        btnsalvar = New Button With {.Text = "Salvar", .Left = 65, .Top = 390, .Width = 100}
        AddHandler btnsalvar.Click, AddressOf BtnSalvar_click

        btnlimpar = New Button With {.Text = "Limpar", .Left = 180, .Top = 390, .Width = 100}
        AddHandler btnlimpar.Click, AddressOf BtnLimpar_click

        btnexcluir = New Button With {.Text = "Excluir", .Left = 295, .Top = 390, .Width = 100}
        AddHandler btnexcluir.Click, AddressOf BtnExcluir_click

        btncontatos = New Button With {.Text = "Contatos", .Left = 410, .Top = 390, .Width = 100}
        AddHandler btncontatos.Click, Sub()
                                          If clienteSelecionadoId > 0 Then
                                              Dim form = New ContatoForm(clienteSelecionadoId, _clienteservice)
                                              form.ShowDialog()
                                          Else
                                              MessageBox.Show("selecione um cliente primeiro.")
                                          End If
                                      End Sub

        btnenderecos = New Button With {.Text = "Endereços", .Left = 525, .Top = 390, .Width = 100}
        AddHandler btnenderecos.Click, Sub()
                                           If clienteSelecionadoId > 0 Then
                                               Dim form = New EnderecoForm(clienteSelecionadoId, _clienteservice)
                                               form.ShowDialog()
                                           Else
                                               MessageBox.Show("selecione um cliente primeiro.")
                                           End If
                                       End Sub

        linkapiurl = New LinkLabel With {
            .Left = 10,
            .Top = 430,
            .AutoSize = True,
            .Text = "api rodando em: http://localhost:8085",
            .LinkColor = Color.DarkBlue
        }
        AddHandler linkapiurl.Click, AddressOf AbrirApinoNavegador


        Me.Controls.AddRange({
            lblfiltronome, txtfiltronome,
            lblfiltroemail, txtfiltroemail,
            lblfiltrocpf, txtfiltrocpf,
            btnfiltrar, dgvclientes,
            lblnome, txtnome,
            lblemail, txtemail,
            lblcpf, txtcpf,
            lblrg, txtrg,
            btnsalvar, btnlimpar, btnexcluir,
            btncontatos, btnenderecos,
            linkapiurl
        })
    End Sub

    Private Sub AbrirApinoNavegador(sender As Object, e As EventArgs)
        Dim url As String = "http://localhost:8085/cliente/listar"
        Try
            Process.Start(New ProcessStartInfo With {
                .FileName = url,
                .UseShellExecute = True
            })
        Catch ex As Exception
            MessageBox.Show("não foi possível abrir o navegador: " & ex.Message)
        End Try
    End Sub

    Private Sub CarregarClientes()
        clientes = _clienteservice.Listar(
            txtfiltronome.Text.Trim(),
            txtfiltroemail.Text.Trim(),
            txtfiltrocpf.Text.Trim()
        )
        dgvclientes.DataSource = clientes.Select(Function(c) New With {
            .id = c.Id,
            .nome = c.Nome,
            .email = c.Email,
            .cpf = c.CPF,
            .rg = c.RG
        }).ToList()
    End Sub

    Private Sub BtnFiltrar_click(sender As Object, e As EventArgs)
        CarregarClientes()
    End Sub

    Private Sub DgvClientes_cellclick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            Dim row = dgvclientes.Rows(e.RowIndex)
            clienteSelecionadoId = Convert.ToInt32(row.Cells("id").Value)
            txtnome.Text = row.Cells("nome").Value.ToString()
            txtemail.Text = row.Cells("email").Value.ToString()
            txtcpf.Text = row.Cells("cpf").Value.ToString()
            txtrg.Text = row.Cells("rg").Value.ToString()
        End If
    End Sub

    Private Sub BtnSalvar_click(sender As Object, e As EventArgs)
        Dim erros = ClienteFormHelper.ValidarCampos(txtnome.Text, txtemail.Text, txtcpf.Text, txtrg.Text)

        If Not String.IsNullOrEmpty(erros) Then
            MessageBox.Show(erros)
            Return
        End If

        Dim cliente As Cliente

        If clienteSelecionadoId = 0 Then
            cliente = New Cliente With {
                .Nome = txtnome.Text,
                .Email = txtemail.Text,
                .CPF = txtcpf.Text,
                .RG = txtrg.Text
            }
            _clienteservice.Criar(cliente)
        Else
            cliente = _clienteservice.ObterPorId(clienteSelecionadoId)
            If cliente Is Nothing Then
                MessageBox.Show("cliente não encontrado.")
                Return
            End If

            cliente.Nome = txtnome.Text
            cliente.Email = txtemail.Text
            cliente.CPF = txtcpf.Text
            cliente.RG = txtrg.Text
            _clienteservice.Atualizar(cliente.Id, cliente)
        End If

        LimparCampos()
        CarregarClientes()
    End Sub

    Private Sub BtnLimpar_click(sender As Object, e As EventArgs)
        limparcampos()
    End Sub

    Private Sub LimparCampos()
        txtnome.Clear()
        txtemail.Clear()
        txtcpf.Clear()
        txtrg.Clear()
        clienteSelecionadoId = 0
        dgvclientes.ClearSelection()
    End Sub

    Private Sub BtnExcluir_click(sender As Object, e As EventArgs)
        If clienteSelecionadoId = 0 Then
            MessageBox.Show("selecione um cliente para excluir.")
            Return
        End If

        If MessageBox.Show("confirma exclusão?", "excluir", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            _clienteservice.Remover(clienteSelecionadoId)
            LimparCampos()
            CarregarClientes()
        End If
    End Sub

    Private api As New ApiHost()

    Protected Overrides Sub onload(e As EventArgs)
        MyBase.OnLoad(e)
        api.Iniciar()
    End Sub

    Protected Overrides Sub onformclosing(e As FormClosingEventArgs)
        api.Parar()
        MyBase.OnFormClosing(e)
    End Sub

End Class
