Public Interface IClienteRepository
    Function Listar() As List(Of Cliente)
    Function ObterPorId(id As Integer) As Cliente
    Sub Criar(cliente As Cliente)
    Sub Atualizar(cliente As Cliente)
    Sub Remover(id As Integer)
End Interface