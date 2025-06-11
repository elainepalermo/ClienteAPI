Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json

Public Class ApiHost
    Private listener As HttpListener
    Private ReadOnly clienteService As IClienteService = New ClienteService(New JsonClienteRepository())

    Public Sub Iniciar()
        listener = New HttpListener()
        listener.Prefixes.Add("http://localhost:8085/")
        listener.Start()
        listener.BeginGetContext(AddressOf ProcessarRequisicao, Nothing)
    End Sub

    Private Sub ProcessarRequisicao(ar As IAsyncResult)
        If listener Is Nothing OrElse Not listener.IsListening Then Return

        Dim context = listener.EndGetContext(ar)
        listener.BeginGetContext(AddressOf ProcessarRequisicao, Nothing)

        Dim request = context.Request
        Dim response = context.Response
        response.ContentType = "application/json"

        Try
            Dim caminho = request.Url.AbsolutePath.ToLower()
            Dim metodo = request.HttpMethod.ToUpper()
            Dim id As Integer = 0
            If caminho.StartsWith("/cliente/atualizar/") OrElse caminho.StartsWith("/cliente/remover/") Then
                Integer.TryParse(caminho.Split("/"c).Last(), id)
            End If

            Select Case True
                Case caminho = "/cliente/listar" AndAlso metodo = "GET"
                    Dim lista = clienteService.Listar(Nothing, Nothing, Nothing)
                    EscreverResposta(response, lista)

                Case caminho = "/cliente/criar" AndAlso metodo = "POST"
                    Dim body = New StreamReader(request.InputStream).ReadToEnd()
                    Dim cliente = JsonConvert.DeserializeObject(Of Cliente)(body)
                    clienteService.Criar(cliente)
                    EscreverResposta(response, New With {.mensagem = "Cliente criado."})

                Case caminho.StartsWith("/cliente/atualizar/") AndAlso metodo = "PUT"
                    Dim body = New StreamReader(request.InputStream).ReadToEnd()
                    Dim Cliente = JsonConvert.DeserializeObject(Of Cliente)(body)
                    clienteService.Atualizar(id, cliente)
                    EscreverResposta(response, New With {.mensagem = "Cliente atualizado."})

                Case caminho.StartsWith("/cliente/remover/") AndAlso metodo = "DELETE"
                    clienteService.Remover(id)
                    EscreverResposta(response, New With {.mensagem = "Cliente removido."})

                Case Else
                    response.StatusCode = 404
                    EscreverResposta(response, New With {.erro = "Rota não encontrada."})
            End Select
        Catch ex As Exception
            response.StatusCode = 500
            EscreverResposta(response, New With {.erro = ex.Message})
        End Try
    End Sub

    Private Sub EscreverResposta(response As HttpListenerResponse, objeto As Object)
        Dim json = JsonConvert.SerializeObject(objeto)
        Dim buffer = Encoding.UTF8.GetBytes(json)
        response.ContentLength64 = buffer.Length
        Using output = response.OutputStream
            output.Write(buffer, 0, buffer.Length)
        End Using
    End Sub

    Public Sub Parar()
        If listener IsNot Nothing Then
            listener.Stop()
            listener.Close()
            listener = Nothing
        End If
    End Sub
End Class