Imports System.Windows.Forms

Public Class EnderecoForm
    Inherits Form

    Private ReadOnly clienteId As Integer
    Private ReadOnly _clienteService As IClienteService
    Private cliente As Cliente
    Private enderecoEditandoId As Integer = 0

    Private panelTopo As Panel
    Private dgvEnderecos As DataGridView

    Private cmbTipo As ComboBox
    Private txtCEP As TextBox
    Private txtLogradouro As TextBox
    Private txtNumero As TextBox
    Private txtBairro As TextBox
    Private txtComplemento As TextBox
    Private txtCidade As TextBox
    Private txtEstado As TextBox
    Private txtReferencia As TextBox

    Private btnSalvar As Button
    Private btnLimpar As Button
    Private btnExcluir As Button

    Public Sub New(clienteId As Integer, clienteService As IClienteService)
        Me.clienteId = clienteId
        Me._clienteService = clienteService
        Me.cliente = _clienteService.ObterPorId(clienteId)

        If Me.cliente.Enderecos Is Nothing Then
            Me.cliente.Enderecos = New List(Of Endereco)()
        End If

        InicializarComponentes()
        CarregarEnderecos()
    End Sub


    Private Sub InicializarComponentes()
        Me.Text = "Endereços do Cliente"
        Me.Size = New Drawing.Size(900, 500)

        panelTopo = New Panel With {.Dock = DockStyle.Top, .Height = 180, .AutoScroll = True}
        dgvEnderecos = New DataGridView With {
            .Dock = DockStyle.Fill,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        }

        Dim lblTipo = New Label With {.Text = "Tipo", .Left = 10, .Top = 10, .Width = 60}
        cmbTipo = New ComboBox With {.Left = 70, .Top = 8, .Width = 150}
        cmbTipo.Items.AddRange(New String() {"Preferencial", "Entrega", "Cobrança"})

        Dim lblCEP = New Label With {.Text = "CEP", .Left = 240, .Top = 10, .Width = 40}
        txtCEP = New TextBox With {.Left = 280, .Top = 8, .Width = 120}

        Dim lblLogradouro = New Label With {.Text = "Logradouro", .Left = 410, .Top = 10, .Width = 80}
        txtLogradouro = New TextBox With {.Left = 490, .Top = 8, .Width = 280}


        Dim lblNumero = New Label With {.Text = "Número", .Left = 10, .Top = 45, .Width = 60}
        txtNumero = New TextBox With {.Left = 70, .Top = 43, .Width = 100}

        Dim lblBairro = New Label With {.Text = "Bairro", .Left = 190, .Top = 45, .Width = 60}
        txtBairro = New TextBox With {.Left = 250, .Top = 43, .Width = 180}

        Dim lblComplemento = New Label With {.Text = "Complemento", .Left = 440, .Top = 45, .Width = 90}
        txtComplemento = New TextBox With {.Left = 530, .Top = 43, .Width = 240}


        Dim lblCidade = New Label With {.Text = "Cidade", .Left = 10, .Top = 80, .Width = 60}
        txtCidade = New TextBox With {.Left = 70, .Top = 78, .Width = 200}

        Dim lblEstado = New Label With {.Text = "Estado", .Left = 290, .Top = 80, .Width = 60}
        txtEstado = New TextBox With {.Left = 350, .Top = 78, .Width = 100}

        Dim lblReferencia = New Label With {.Text = "Referência", .Left = 470, .Top = 80, .Width = 80}
        txtReferencia = New TextBox With {.Left = 555, .Top = 78, .Width = 215}

        btnSalvar = New Button With {.Text = "Salvar", .Left = 70, .Top = 120, .Width = 100}
        AddHandler btnSalvar.Click, AddressOf BtnSalvar_Click

        btnLimpar = New Button With {.Text = "Limpar", .Left = 190, .Top = 120, .Width = 100}
        AddHandler btnLimpar.Click, AddressOf BtnLimpar_Click

        btnExcluir = New Button With {.Text = "Excluir", .Left = 310, .Top = 120, .Width = 100}
        AddHandler btnExcluir.Click, AddressOf BtnExcluir_Click

        panelTopo.Controls.AddRange({
            lblTipo, cmbTipo, lblCEP, txtCEP, lblLogradouro, txtLogradouro,
            lblNumero, txtNumero, lblBairro, txtBairro, lblComplemento, txtComplemento,
            lblCidade, txtCidade, lblEstado, txtEstado, lblReferencia, txtReferencia,
            btnSalvar, btnLimpar, btnExcluir
        })

        AddHandler dgvEnderecos.CellClick, AddressOf dgvEnderecos_CellClick

        Me.Controls.Add(dgvEnderecos)
        Me.Controls.Add(panelTopo)
    End Sub

    Private Sub CarregarEnderecos()
        dgvEnderecos.DataSource = Nothing

        If cliente IsNot Nothing AndAlso cliente.Enderecos IsNot Nothing Then
            dgvEnderecos.DataSource = cliente.Enderecos.Select(Function(e) New With {
            e.Id, e.Tipo, e.CEP, e.Logradouro, e.Numero,
            e.Bairro, e.Complemento, e.Cidade, e.Estado, e.Referencia
        }).ToList()
        Else
            dgvEnderecos.DataSource = New List(Of Object)()
        End If
    End Sub


    Private Sub BtnSalvar_Click(sender As Object, e As EventArgs)
        Try
            Dim tipo = cmbTipo.SelectedItem?.ToString()
            If Not EnderecoHelper.ValidarEndereco(tipo, txtNumero.Text, txtCEP.Text) Then
                Return
            End If

            Dim numero = Integer.Parse(txtNumero.Text)

            If enderecoEditandoId > 0 Then
                Dim endereco = cliente.Enderecos.FirstOrDefault(Function(c) c.Id = enderecoEditandoId)
                If endereco IsNot Nothing Then
                    EnderecoHelper.PreencherEndereco(endereco, tipo, txtCEP.Text, txtLogradouro.Text,
                                                   numero, txtBairro.Text, txtComplemento.Text,
                                                   txtCidade.Text, txtEstado.Text, txtReferencia.Text)
                End If
            Else
                Dim novoId = If(cliente.Enderecos.Count = 0, 1, cliente.Enderecos.Max(Function(c) c.Id) + 1)
                Dim novoEndereco = New Endereco With {.Id = novoId}
                EnderecoHelper.PreencherEndereco(novoEndereco, tipo, txtCEP.Text, txtLogradouro.Text,
                                                numero, txtBairro.Text, txtComplemento.Text,
                                                txtCidade.Text, txtEstado.Text, txtReferencia.Text)
                cliente.Enderecos.Add(novoEndereco)
            End If

            _clienteService.Atualizar(cliente.Id, cliente)
            enderecoEditandoId = 0
            EnderecoHelper.LimparCampos(cmbTipo, txtCEP, txtLogradouro, txtNumero, txtBairro, txtComplemento, txtCidade, txtEstado, txtReferencia)
            CarregarEnderecos()
        Catch ex As Exception
            MessageBox.Show("Erro: " & ex.Message)
        End Try
    End Sub

    Private Sub BtnExcluir_Click(sender As Object, e As EventArgs)
        If enderecoEditandoId > 0 Then
            cliente.Enderecos.RemoveAll(Function(c) c.Id = enderecoEditandoId)
            _clienteService.Atualizar(cliente.Id, cliente)
            enderecoEditandoId = 0
            EnderecoHelper.LimparCampos(cmbTipo, txtCEP, txtLogradouro, txtNumero, txtBairro, txtComplemento, txtCidade, txtEstado, txtReferencia)
            CarregarEnderecos()
        Else
            MessageBox.Show("Selecione um endereço para excluir.")
        End If
    End Sub

    Private Sub BtnLimpar_Click(sender As Object, e As EventArgs)
        enderecoEditandoId = 0
        EnderecoHelper.LimparCampos(cmbTipo, txtCEP, txtLogradouro, txtNumero, txtBairro, txtComplemento, txtCidade, txtEstado, txtReferencia)
    End Sub

    Private Sub dgvEnderecos_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            Dim linha = dgvEnderecos.Rows(e.RowIndex)
            enderecoEditandoId = Convert.ToInt32(linha.Cells("Id").Value)
            Dim endereco = cliente.Enderecos.FirstOrDefault(Function(c) c.Id = enderecoEditandoId)
            If endereco IsNot Nothing Then
                EnderecoHelper.PreencherCamposDoForm(endereco, cmbTipo, txtCEP, txtLogradouro, txtNumero, txtBairro, txtComplemento, txtCidade, txtEstado, txtReferencia)
            End If
        End If
    End Sub
End Class
