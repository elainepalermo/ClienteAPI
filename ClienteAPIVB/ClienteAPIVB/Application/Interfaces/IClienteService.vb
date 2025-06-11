Public Interface IClienteService
    Function Listar(nome As String, email As String, cpf As String) As List(Of Cliente)
    Function ObterPorId(id As Integer) As Cliente
    Sub Criar(cliente As Cliente)
    Sub Atualizar(id As Integer, cliente As Cliente)
    Sub Remover(id As Integer)
End Interface