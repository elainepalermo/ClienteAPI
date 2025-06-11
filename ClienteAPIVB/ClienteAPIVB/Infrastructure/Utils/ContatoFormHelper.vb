Public Class ContatoHelper

    Public Shared Function ValidarContato(tipo As String, dddStr As String, telefoneStr As String, ByRef erro As String) As Boolean
        erro = ""

        If String.IsNullOrEmpty(tipo) Then
            erro = "Tipo de contato é obrigatório."
            Return False
        End If

        Dim ddd As Integer
        If Not Integer.TryParse(dddStr, ddd) OrElse ddd < 10 OrElse ddd > 99 Then
            erro = "DDD inválido. Deve conter dois números."
            Return False
        End If

        Dim telefone As Long
        If Not Long.TryParse(telefoneStr, telefone) OrElse telefone <= 0 Then
            erro = "Telefone inválido."
            Return False
        End If

        Return True
    End Function

    Public Shared Function CriarContato(tipo As String, ddd As Integer, telefone As Long, Optional id As Integer = 0) As Contato
        Return New Contato With {
            .Id = id,
            .Tipo = tipo,
            .DDD = ddd,
            .Telefone = telefone
        }
    End Function

End Class
