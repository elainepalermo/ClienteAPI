Public Class ClienteService
    Implements IClienteService

    Private ReadOnly _repo As IClienteRepository

    Public Sub New(repo As IClienteRepository)
        _repo = repo
    End Sub

    Public Function Listar(nome As String, email As String, cpf As String) As List(Of Cliente) Implements IClienteService.Listar
        Dim lista = _repo.Listar()
        If Not String.IsNullOrWhiteSpace(nome) Then
            lista = lista.Where(Function(c) c.Nome.Contains(nome)).ToList()
        End If
        If Not String.IsNullOrWhiteSpace(email) Then
            lista = lista.Where(Function(c) c.Email.Contains(email)).ToList()
        End If
        If Not String.IsNullOrWhiteSpace(cpf) Then
            lista = lista.Where(Function(c) c.CPF.Contains(cpf)).ToList()
        End If
        Return lista
    End Function

    Public Function ObterPorId(id As Integer) As Cliente Implements IClienteService.ObterPorId
        Return _repo.ObterPorId(id)
    End Function

    Public Sub Criar(cliente As Cliente) Implements IClienteService.Criar
        _repo.Criar(cliente)
    End Sub

    Public Sub Atualizar(id As Integer, cliente As Cliente) Implements IClienteService.Atualizar
        cliente.Id = id
        _repo.Atualizar(cliente)
    End Sub

    Public Sub Remover(id As Integer) Implements IClienteService.Remover
        _repo.Remover(id)
    End Sub
End Class