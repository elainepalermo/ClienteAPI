Public Class Cliente
    Public Property Id As Integer
    Public Property Nome As String
    Public Property Email As String
    Public Property CPF As String
    Public Property RG As String
    Public Property Contatos As List(Of Contato)
    Public Property Enderecos As List(Of Endereco)
End Class
