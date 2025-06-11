Module Program
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Dim clienteService As IClienteService = Startup.ResolveClienteService()
        Application.Run(New ClienteForm(clienteService))
    End Sub
End Module
