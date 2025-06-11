Imports System.Web.Http

<RoutePrefix("cliente")>
Public Class ClienteController
    Inherits ApiController

    Private ReadOnly _service As IClienteService

    Public Sub New()
        _service = New ClienteService(New JsonClienteRepository())
    End Sub

    <HttpGet>
    <Route("listar")>
    Public Function listar() As IHttpActionResult
        Dim clientes = _service.Listar(Nothing, Nothing, Nothing)
        Return Ok(clientes)
    End Function

    <HttpPost>
    <Route("criar")>
    Public Function criar(<FromBody> cliente As Cliente) As IHttpActionResult
        _service.Criar(cliente)
        Return Ok()
    End Function

    <HttpPut>
    <Route("atualizar/{id}")>
    Public Function atualizar(id As Integer, <FromBody> cliente As Cliente) As IHttpActionResult
        _service.Atualizar(id, cliente)
        Return Ok()
    End Function

    <HttpDelete>
    <Route("remover/{id}")>
    Public Function remover(id As Integer) As IHttpActionResult
        _service.Remover(id)
        Return Ok()
    End Function
End Class
