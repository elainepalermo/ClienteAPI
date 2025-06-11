Imports System.Text.RegularExpressions
Imports System.Windows.Forms

Public Module EnderecoHelper

    Public Function ValidarCEP(cep As String) As Boolean
        If String.IsNullOrEmpty(cep) Then
            MessageBox.Show("CEP é obrigatório.")
            Return False
        End If

        Dim regex As New Regex("^\d{5}-\d{3}$")
        If Not regex.IsMatch(cep) Then
            MessageBox.Show("CEP inválido. O formato correto é xxxxx-xxx.")
            Return False
        End If

        Return True
    End Function

    Public Function ValidarEndereco(tipo As String, numeroTexto As String, Optional cep As String = Nothing) As Boolean
        If String.IsNullOrEmpty(tipo) Then
            MessageBox.Show("Selecione o tipo de endereço.")
            Return False
        End If

        Dim numero As Integer
        If Not Integer.TryParse(numeroTexto, numero) OrElse numero <= 0 Then
            MessageBox.Show("Número inválido.")
            Return False
        End If

        If cep IsNot Nothing Then
            If Not ValidarCEP(cep) Then
                Return False
            End If
        End If

        Return True
    End Function

    Public Sub PreencherEndereco(endereco As Endereco, tipo As String, cep As String, logradouro As String,
                                 numero As Integer, bairro As String, complemento As String,
                                 cidade As String, estado As String, referencia As String)
        endereco.Tipo = tipo
        endereco.CEP = cep
        endereco.Logradouro = logradouro
        endereco.Numero = numero
        endereco.Bairro = bairro
        endereco.Complemento = complemento
        endereco.Cidade = cidade
        endereco.Estado = estado
        endereco.Referencia = referencia
    End Sub

    Public Sub LimparCampos(cmbTipo As ComboBox, txtCEP As TextBox, txtLogradouro As TextBox,
                            txtNumero As TextBox, txtBairro As TextBox, txtComplemento As TextBox,
                            txtCidade As TextBox, txtEstado As TextBox, txtReferencia As TextBox)
        cmbTipo.SelectedIndex = -1
        txtCEP.Clear()
        txtLogradouro.Clear()
        txtNumero.Clear()
        txtBairro.Clear()
        txtComplemento.Clear()
        txtCidade.Clear()
        txtEstado.Clear()
        txtReferencia.Clear()
    End Sub

    Public Sub PreencherCamposDoForm(endereco As Endereco, cmbTipo As ComboBox, txtCEP As TextBox,
                                     txtLogradouro As TextBox, txtNumero As TextBox, txtBairro As TextBox,
                                     txtComplemento As TextBox, txtCidade As TextBox,
                                     txtEstado As TextBox, txtReferencia As TextBox)
        cmbTipo.SelectedItem = endereco.Tipo
        txtCEP.Text = endereco.CEP
        txtLogradouro.Text = endereco.Logradouro
        txtNumero.Text = endereco.Numero.ToString()
        txtBairro.Text = endereco.Bairro
        txtComplemento.Text = endereco.Complemento
        txtCidade.Text = endereco.Cidade
        txtEstado.Text = endereco.Estado
        txtReferencia.Text = endereco.Referencia
    End Sub

End Module
