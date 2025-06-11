Public Class Startup
    Public Shared Function ResolveClienteService() As IClienteService
        Return New ClienteService(New JsonClienteRepository())
    End Function
End Class
