Imports System.IO
Imports System.Xml
Imports Newtonsoft.Json

Public Class JsonClienteRepository
    Implements IClienteRepository

    Private ReadOnly _filePath As String = "clientes.json"

    Public Function Listar() As List(Of Cliente) Implements IClienteRepository.Listar
        If Not File.Exists(_filePath) Then
            Return New List(Of Cliente)()
        End If
        Dim json = File.ReadAllText(_filePath)
        Return JsonConvert.DeserializeObject(Of List(Of Cliente))(json)
    End Function

    Public Function ObterPorId(id As Integer) As Cliente Implements IClienteRepository.ObterPorId
        Return Listar().FirstOrDefault(Function(c) c.Id = id)
    End Function

    Public Sub Criar(cliente As Cliente) Implements IClienteRepository.Criar
        Dim clientes = Listar()
        cliente.Id = If(clientes.Count = 0, 1, clientes.Max(Function(c) c.Id) + 1)
        clientes.Add(cliente)
        Salvar(clientes)
    End Sub

    Public Sub Atualizar(cliente As Cliente) Implements IClienteRepository.Atualizar
        Dim clientes = Listar()
        Dim index = clientes.FindIndex(Function(c) c.Id = cliente.Id)
        If index >= 0 Then
            clientes(index) = cliente
            Salvar(clientes)
        End If
    End Sub

    Public Sub Remover(id As Integer) Implements IClienteRepository.Remover
        Dim clientes = Listar()
        clientes.RemoveAll(Function(c) c.Id = id)
        Salvar(clientes)
    End Sub

    Private Sub Salvar(clientes As List(Of Cliente))
        Dim json = JsonConvert.SerializeObject(clientes, Newtonsoft.Json.Formatting.Indented)
        File.WriteAllText(_filePath, json)
    End Sub
End Class