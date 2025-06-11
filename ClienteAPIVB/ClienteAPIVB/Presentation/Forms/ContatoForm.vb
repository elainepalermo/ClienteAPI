Imports System.Windows.Forms

Public Class ContatoForm
    Inherits Form

    Private ReadOnly _clienteId As Integer
    Private ReadOnly _clienteService As IClienteService
    Private cliente As Cliente
    Private contatoEditandoId As Integer = 0


    Private panelTopo As Panel
    Private lblTipo As Label
    Private cmbTipo As ComboBox
    Private lblDDD As Label
    Private txtDDD As TextBox
    Private lblTelefone As Label
    Private txtTelefone As TextBox
    Private btnSalvar As Button
    Private btnLimpar As Button
    Private btnExcluir As Button
    Private dgvContatos As DataGridView

    Public Sub New(clienteId As Integer, clienteService As IClienteService)
        Me._clienteId = clienteId
        Me._clienteService = clienteService
        Me.cliente = _clienteService.ObterPorId(clienteId)

        InicializarComponentes()
        CarregarContatos()
    End Sub

    Private Sub InicializarComponentes()
        Me.Text = $"Contatos do Cliente: {cliente.Nome}"
        Me.Size = New Drawing.Size(600, 400)
        Me.StartPosition = FormStartPosition.CenterParent

        panelTopo = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 130
        }


        lblTipo = New Label With {
            .Text = "Tipo",
            .Left = 10,
            .Top = 10,
            .AutoSize = True
        }
        cmbTipo = New ComboBox With {
            .Left = 70,
            .Top = 8,
            .Width = 150,
            .DropDownStyle = ComboBoxStyle.DropDownList
        }
        cmbTipo.Items.AddRange(New String() {"Residencial", "Comercial", "Celular"})


        lblDDD = New Label With {
            .Text = "DDD",
            .Left = 250,
            .Top = 10,
            .AutoSize = True
        }
        txtDDD = New TextBox With {
            .Left = 290,
            .Top = 8,
            .Width = 50
        }


        lblTelefone = New Label With {
            .Text = "Telefone",
            .Left = 360,
            .Top = 10,
            .AutoSize = True
        }
        txtTelefone = New TextBox With {
            .Left = 430,
            .Top = 8,
            .Width = 120
        }


        btnSalvar = New Button With {
            .Text = "Salvar",
            .Left = 70,
            .Top = 50,
            .Width = 100
        }
        AddHandler btnSalvar.Click, AddressOf btnSalvar_Click

        btnLimpar = New Button With {
            .Text = "Limpar",
            .Left = 180,
            .Top = 50,
            .Width = 100
        }
        AddHandler btnLimpar.Click, AddressOf btnLimpar_Click

        btnExcluir = New Button With {
            .Text = "Excluir",
            .Left = 290,
            .Top = 50,
            .Width = 100
        }
        AddHandler btnExcluir.Click, AddressOf BtnExcluir_Click

        panelTopo.Controls.Add(lblTipo)
        panelTopo.Controls.Add(cmbTipo)
        panelTopo.Controls.Add(lblDDD)
        panelTopo.Controls.Add(txtDDD)
        panelTopo.Controls.Add(lblTelefone)
        panelTopo.Controls.Add(txtTelefone)
        panelTopo.Controls.Add(btnSalvar)
        panelTopo.Controls.Add(btnLimpar)
        panelTopo.Controls.Add(btnExcluir)


        dgvContatos = New DataGridView With {
            .Dock = DockStyle.Fill,
            .ReadOnly = True,
            .AllowUserToAddRows = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoGenerateColumns = False
        }
        AddHandler dgvContatos.CellClick, AddressOf dgvContatos_CellClick


        dgvContatos.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "Id",
            .Name = "Id",
            .HeaderText = "ID",
            .Width = 50
        })

        dgvContatos.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "Tipo",
            .HeaderText = "Tipo",
            .Width = 150
        })
        dgvContatos.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "DDD",
            .HeaderText = "DDD",
            .Width = 50
        })
        dgvContatos.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "Telefone",
            .HeaderText = "Telefone",
            .Width = 120
        })

        Me.Controls.Add(dgvContatos)
        Me.Controls.Add(panelTopo)
    End Sub

    Private Sub CarregarContatos()
        dgvContatos.DataSource = Nothing
        If cliente.Contatos Is Nothing Then
            cliente.Contatos = New List(Of Contato)()
        End If
        dgvContatos.DataSource = cliente.Contatos.Select(Function(c) New With {
            .Id = c.Id,
            .Tipo = c.Tipo,
            .DDD = c.DDD,
             .Telefone = c.Telefone.ToString("0")
        }).ToList()
    End Sub

    Private Sub dgvContatos_CellClick(sender As Object, e As DataGridViewCellEventArgs)
        If e.RowIndex >= 0 Then
            Dim linha = dgvContatos.Rows(e.RowIndex)
            contatoEditandoId = Convert.ToInt32(linha.Cells("Id").Value)
            Dim contato = cliente.Contatos.FirstOrDefault(Function(c) c.Id = contatoEditandoId)
            If contato IsNot Nothing Then
                cmbTipo.SelectedItem = contato.Tipo
                txtDDD.Text = contato.DDD.ToString()
                txtTelefone.Text = contato.Telefone.ToString("0")
            End If
        End If
    End Sub

    Private Sub btnSalvar_Click(sender As Object, e As EventArgs)
        Dim tipo = cmbTipo.SelectedItem?.ToString()
        Dim erroValidacao As String = ""

        If Not ContatoHelper.ValidarContato(tipo, txtDDD.Text, txtTelefone.Text, erroValidacao) Then
            MessageBox.Show(erroValidacao, "Erro de Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim ddd As Integer = Integer.Parse(txtDDD.Text)
        Dim telefone As Long = Long.Parse(txtTelefone.Text)

        If contatoEditandoId > 0 Then

            Dim contato = cliente.Contatos.FirstOrDefault(Function(c) c.Id = contatoEditandoId)
            If contato IsNot Nothing Then
                contato.Tipo = tipo
                contato.DDD = ddd
                contato.Telefone = telefone
            End If
        Else

            Dim novoId = If(cliente.Contatos.Count = 0, 1, cliente.Contatos.Max(Function(c) c.Id) + 1)
            Dim novoContato = ContatoHelper.CriarContato(tipo, ddd, telefone, novoId)
            cliente.Contatos.Add(novoContato)
        End If

        _clienteService.Atualizar(cliente.Id, cliente)
        contatoEditandoId = 0
        LimparCampos()
        CarregarContatos()
    End Sub

    Private Sub btnLimpar_Click(sender As Object, e As EventArgs)
        LimparCampos()
    End Sub

    Private Sub LimparCampos()
        cmbTipo.SelectedIndex = -1
        txtDDD.Clear()
        txtTelefone.Clear()
        contatoEditandoId = 0
    End Sub

    Private Sub BtnExcluir_Click(sender As Object, e As EventArgs)
        If contatoEditandoId > 0 Then
            If MessageBox.Show("Deseja realmente excluir este contato?", "Confirmação", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                Dim contato = cliente.Contatos.FirstOrDefault(Function(c) c.Id = contatoEditandoId)
                If contato IsNot Nothing Then
                    cliente.Contatos.Remove(contato)
                    _clienteService.Atualizar(cliente.Id, cliente)
                    LimparCampos()
                    CarregarContatos()
                End If
            End If
        Else
            MessageBox.Show("Selecione um contato para excluir.")
        End If
    End Sub

End Class
