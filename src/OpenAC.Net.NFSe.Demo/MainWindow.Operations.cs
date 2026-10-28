using System.Globalization;
using Avalonia.Platform.Storage;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.DANFSe.PDFSharp;
using OpenAC.Net.NFSe.Nota;

namespace OpenAC.Net.NFSe.Demo;

public partial class MainWindow
{
    private Task RunOperationAsync(string name, Func<CancellationToken, Task> action) => RunAsync(name, action, true);

    private void ConnectOperationsEvents()
    {
        OpenXmlButton.Click += async (_, _) => await RunOperationAsync("Carregar XML", LoadDocumentsAsync);
        GenerateRpsButton.Click += async (_, _) => await RunOperationAsync("Gerar RPS", GenerateRpsAsync);
        ClearDocumentsButton.Click += (_, _) => { client.NotasServico.Clear(); UpdateDocumentCount(); };
        SaveXmlButton.Click += async (_, _) => await RunOperationAsync("Salvar XML", SaveDocumentAsync);
        SendBatchButton.Click += async (_, _) => await RunOperationAsync("Enviar lote", async ct => ShowResponse(await serviceClient.EnviarAsync(BatchNumber(), cancellationToken: ct)));
        SendSyncBatchButton.Click += async (_, _) => await RunOperationAsync("Enviar lote síncrono", async ct => ShowResponse(await serviceClient.EnviarAsync(BatchNumber(), true, ct)));
        QueryNfseButton.Click += async (_, _) => await RunOperationAsync("Consultar NFS-e", async ct => ShowResponse(await serviceClient.ConsultaNFSeAsync(PositiveInt(InvoiceNumberText.Text, "Número da NFS-e"), cancellationToken: ct)));
        QueryStatusButton.Click += async (_, _) => await RunOperationAsync("Consultar situação", ct => QueryBatchAsync(true, ct));
        QueryBatchButton.Click += async (_, _) => await RunOperationAsync("Consultar lote", ct => QueryBatchAsync(false, ct));
        QueryRpsButton.Click += async (_, _) => await RunOperationAsync("Consultar RPS", QueryRpsAsync);
        QueryPeriodButton.Click += async (_, _) => await RunOperationAsync("Consultar período", QueryPeriodAsync);
        QuerySequenceButton.Click += async (_, _) => await RunOperationAsync("Consultar sequencial", async ct =>
        {
            var fields = await InputDialog.AskAsync(this, "Consultar sequencial RPS", new InputField("serie", "Série", "1"));
            if (fields != null) ShowResponse(await serviceClient.ConsultarSequencialRpsAsync(fields["serie"], ct));
        });
        QueryRecipientButton.Click += async (_, _) => await RunOperationAsync("Consultar por tomador", async ct =>
        {
            var fields = await InputDialog.AskAsync(this, "Consultar por tomador", new("documento", "CPF / CNPJ"), new("im", "Inscrição municipal", Required: false));
            if (fields != null) ShowResponse(await serviceClient.ConsultaNFSeTomadorAsync(fields["documento"], fields["im"], cancellationToken: ct));
        });
        QueryAgentButton.Click += async (_, _) => await RunOperationAsync("Consultar por intermediário", async ct =>
        {
            var fields = await InputDialog.AskAsync(this, "Consultar por intermediário", new("nome", "Nome"), new("documento", "CPF / CNPJ"), new("im", "Inscrição municipal", Required: false));
            if (fields != null) ShowResponse(await serviceClient.ConsultaNFSeIntermediarioAsync(fields["nome"], fields["documento"], fields["im"], cancellationToken: ct));
        });
        CancelNfseButton.Click += async (_, _) => await RunOperationAsync("Cancelar NFS-e", ct => CancelInvoiceAsync(false, ct));
        SubstituteButton.Click += async (_, _) => await RunOperationAsync("Substituir NFS-e", ct => CancelInvoiceAsync(true, ct));
        CancelBatchButton.Click += async (_, _) => await RunOperationAsync("Cancelar lote", async ct =>
        {
            RequireDocuments();
            var fields = await InputDialog.AskAsync(this, "Cancelar as notas carregadas", new("motivo", "Motivo do cancelamento"), new("codigo", "Código do cancelamento", "1"));
            if (fields == null) return;
            foreach (var note in client.NotasServico) { note.Cancelamento.MotivoCancelamento = fields["motivo"]; note.Cancelamento.Pedido.CodigoCancelamento = fields["codigo"]; }
            ShowResponse(await serviceClient.CancelarNFSeLoteAsync(BatchNumber(), ct));
        });
        PdfButton.Click += async (_, _) => await RunOperationAsync("Gerar DANFSe", GeneratePdfAsync);
        PreviewButton.Click += async (_, _) => await RunOperationAsync("Visualizar DANFSe", async ct =>
        {
            var notes = RequireDocuments();
            await Task.Run(() => new OpenDANFSePDFSharp(client.Configuracoes).Imprimir(notes), ct);
        });
    }

    private async Task LoadDocumentsAsync(CancellationToken token)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Abrir RPS / NFS-e", AllowMultiple = true,
            FileTypeFilter = [new FilePickerFileType("XML") { Patterns = ["*.xml"] }]
        });
        if (files.Count == 0) return;
        var loaded = new List<NotaServico>();
        foreach (var file in files)
        {
            await using var stream = await file.OpenReadAsync();
            using var reader = new StreamReader(stream);
            var xml = await reader.ReadToEndAsync(token);
            var temporary = new NotaServicoCollection(client.Configuracoes);
            loaded.Add(await Task.Run(() => temporary.Load(xml), token));
        }
        token.ThrowIfCancellationRequested();
        client.NotasServico.Clear();
        foreach (var note in loaded) client.NotasServico.Add(note);
        DocumentNameText.Text = string.Join(", ", files.Select(file => file.Name));
        UpdateDocumentCount();
        ResultText.Text = $"{loaded.Count} documento(s) carregado(s).";
    }

    private async Task SaveDocumentAsync(CancellationToken token)
    {
        var notes = RequireDocuments();
        if (notes.Length == 1)
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Salvar XML", SuggestedFileName = "NFSe.xml", DefaultExtension = "xml",
                FileTypeChoices = [new FilePickerFileType("XML") { Patterns = ["*.xml"] }]
            });
            if (file == null) return;
            var xml = await Task.Run(notes[0].GetXml, token);
            await using var stream = await file.OpenWriteAsync();
            stream.SetLength(0);
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(xml.AsMemory(), token);
        }
        else
        {
            var folder = (await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Salvar XMLs" })).FirstOrDefault()?.TryGetLocalPath();
            if (folder == null) return;
            foreach (var note in notes) { token.ThrowIfCancellationRequested(); await Task.Run(() => note.Save(folder), token); }
        }
        ResultText.Text = "Documentos salvos.";
    }

    private async Task GeneratePdfAsync(CancellationToken token)
    {
        var notes = RequireDocuments();
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Salvar DANFSe", SuggestedFileName = "DANFSe.pdf", DefaultExtension = "pdf",
            FileTypeChoices = [new FilePickerFileType("PDF") { Patterns = ["*.pdf"] }]
        });
        if (file == null) return;
        var bytes = await Task.Run(() => new OpenDANFSePDFSharp(client.Configuracoes).GerarPDF(notes), token);
        await using var stream = await file.OpenWriteAsync();
        stream.SetLength(0);
        await stream.WriteAsync(bytes, token);
        ResultText.Text = "DANFSe salvo: " + file.Name;
    }

    private async Task QueryBatchAsync(bool status, CancellationToken token)
    {
        var fields = await InputDialog.AskAsync(this, status ? "Situação do lote" : "Consultar lote",
            new("lote", "Número do lote", BatchText.Text ?? "1"), new("protocolo", "Protocolo", Required: !status));
        if (fields == null) return;
        var batch = PositiveInt(fields["lote"], "Lote");
        ShowResponse(status ? await serviceClient.ConsultarSituacaoAsync(batch, fields["protocolo"], token)
            : await serviceClient.ConsultarLoteRpsAsync(batch, fields["protocolo"], token));
    }

    private async Task QueryRpsAsync(CancellationToken token)
    {
        var fields = await InputDialog.AskAsync(this, "Consultar NFS-e por RPS", new("numero", "Número do RPS", "1"),
            new("serie", "Série", "1"), new("tipo", "Tipo (RPS, RPSM ou RPSC)", "RPS"),
            new("mes", "Mês de competência", DateTime.Today.Month.ToString()), new("ano", "Ano de competência", DateTime.Today.Year.ToString()));
        if (fields == null) return;
        if (!Enum.TryParse<TipoRps>(fields["tipo"], true, out var type) || !Enum.IsDefined(type)) throw new ArgumentException("Tipo de RPS inválido.");
        ShowResponse(await serviceClient.ConsultaNFSeRpsAsync(PositiveInt(fields["numero"], "RPS"), fields["serie"], type,
            PositiveInt(fields["mes"], "Mês"), PositiveInt(fields["ano"], "Ano"), token));
    }

    private async Task QueryPeriodAsync(CancellationToken token)
    {
        var fields = await InputDialog.AskAsync(this, "Consultar NFS-e por período",
            new("inicio", "Data inicial (dd/MM/aaaa)", DateTime.Today.AddDays(-30).ToString("dd/MM/yyyy")),
            new("fim", "Data final (dd/MM/aaaa)", DateTime.Today.ToString("dd/MM/yyyy")), new("pagina", "Página", "1"));
        if (fields == null) return;
        var start = DateTime.ParseExact(fields["inicio"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
        var end = DateTime.ParseExact(fields["fim"], "dd/MM/yyyy", CultureInfo.InvariantCulture);
        ShowResponse(await serviceClient.ConsultaNFSePeriodoAsync(start, end, PositiveInt(fields["pagina"], "Página"), token));
    }

    private async Task CancelInvoiceAsync(bool substitute, CancellationToken token)
    {
        if (substitute) RequireDocuments();
        var fields = await InputDialog.AskAsync(this, substitute ? "Confirmar substituição da NFS-e" : "Confirmar cancelamento da NFS-e",
            new("numero", "Número da NFS-e", InvoiceNumberText.Text ?? ""), new("codigo", "Código do cancelamento", "1"),
            new("motivo", "Motivo"), new("serie", "Série", Required: false), new("valor", "Valor da NFS-e", "0"),
            new("verificacao", "Código de verificação", Required: false));
        if (fields == null) return;
        ShowResponse(substitute ? await serviceClient.SubstituirNFSeAsync(fields["codigo"], fields["numero"], fields["motivo"], token)
            : await serviceClient.CancelarNFSeAsync(fields["codigo"], fields["numero"], fields["serie"], decimal.Parse(fields["valor"], CultureInfo.CurrentCulture), fields["motivo"], fields["verificacao"], token));
    }

    private NotaServico[] RequireDocuments() => client.NotasServico.Count > 0 ? client.NotasServico.ToArray()
        : throw new ArgumentException("Carregue um XML ou gere um RPS primeiro.");
    private void UpdateDocumentCount()
    {
        DocumentCountText.Text = $"{client.NotasServico.Count} documento(s) carregado(s)";
        if (client.NotasServico.Count == 0) DocumentNameText.Text = "Nenhum XML carregado";
        else if (string.IsNullOrWhiteSpace(DocumentNameText.Text) || DocumentNameText.Text == "Nenhum XML carregado") DocumentNameText.Text = "RPS / NFS-e";
    }
    private int BatchNumber() => PositiveInt(BatchText.Text, "Número do lote");
    private static int PositiveInt(string? value, string label) => int.TryParse(value, out var number) && number > 0
        ? number : throw new ArgumentException(label + " deve ser um número inteiro positivo.");
}
