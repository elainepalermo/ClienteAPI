Imports System.Text.RegularExpressions

Public Module ClienteFormHelper
    Public Function ValidarCampos(nome As String, email As String, cpf As String, rg As String) As String
        Dim erros As New List(Of String)

        If String.IsNullOrWhiteSpace(nome) Then
            erros.Add("O campo Nome é obrigatório.")
        End If

        If String.IsNullOrWhiteSpace(email) Then
            erros.Add("O campo Email é obrigatório.")
        End If

        If Not EmailEhValido(email) Then
            erros.Add("E-mail inválido.")
        End If

        If Not ValidarCPF(cpf) Then
            erros.Add("CPF inválido. Formato: XXX.XXX.XXX-XX")
        End If

        If Not ValidarRG(rg) Then
            erros.Add("RG inválido. Formato: XX.XXX.XXX-X.")
        End If

        Return String.Join(Environment.NewLine, erros)
    End Function


    Public Function MontarCliente(id As Integer, nome As String, email As String, cpf As String, rg As String) As Cliente
        Return New Cliente With {
            .Id = id,
            .Nome = nome.Trim(),
            .Email = email.Trim(),
            .CPF = cpf.Trim(),
            .RG = rg.Trim(),
            .Contatos = New List(Of Contato)(),
            .Enderecos = New List(Of Endereco)()
        }
    End Function

    Private Function EmailEhValido(email As String) As Boolean
        Dim pattern As String = "^[^@\s]+@[^@\s]+\.[^@\s]+$"
        Return System.Text.RegularExpressions.Regex.IsMatch(email, pattern)
    End Function

    Public Function ValidarCPF(cpf As String) As Boolean
        Dim cpfPattern As String = "^\d{3}\.\d{3}\.\d{3}-\d{2}$"
        Return Regex.IsMatch(cpf, cpfPattern)
    End Function


    Public Function ValidarRG(rg As String) As Boolean
        Dim rgPattern As String = "^\d{2}\.\d{3}\.\d{3}-[a-zA-Z0-9]$"
        Return Regex.IsMatch(rg, rgPattern)
    End Function

End Module
