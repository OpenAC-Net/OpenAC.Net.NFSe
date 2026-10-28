using Avalonia.Controls;
using Avalonia.Platform.Storage;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.Core.Extensions;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Demo;

public partial class MainWindow
{
    private string DataDirectory => Path.GetDirectoryName(configurationPath)!;
    private void InitializeSettings(string? configurationFile)
    {
        configurationPath = configurationFile ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenAC.Net", "NFSe.Demo", "opennfse.config");
        Directory.CreateDirectory(DataDirectory);
        if (!File.Exists(configurationPath) && File.Exists("opennfse.config")) File.Copy("opennfse.config", configurationPath);
        configuration = OpenConfig.CreateOrLoad(configurationPath);
        ConfigurationPathText.Text = "Configuração local: " + configurationPath;
        Fill(EnvironmentCombo, new[] { DFeTipoAmbiente.Homologacao, DFeTipoAmbiente.Producao }, configuration.Get("Ambiente", DFeTipoAmbiente.Homologacao));
        catalogPath = Path.Combine(DataDirectory, "Municipios.nfse");
        var legacyCatalog = configuration.Get("ArquivoCidades", "");
        if (File.Exists(catalogPath)) ProviderManager.Load(catalogPath);
        else if (File.Exists(legacyCatalog)) ProviderManager.Load(legacyCatalog);
        MunicipalityInput.Value = configuration.Get("Municipio", ProviderManager.Municipios.First().Codigo);
        TimeoutInput.Value = Math.Clamp(configuration.Get("Timeout", 100), 1, 600);
        SaveDocumentsCheck.IsChecked = configuration.Get("SalvarNfse", "0") == "1";
        SaveExchangeCheck.IsChecked = configuration.Get("SalvarEnvios", false);
        CertificatePathText.Text = configuration.Get("Certificado", "");
        CertificateSerialText.Text = configuration.Get("NumeroSerie", "");
        TaxIdText.Text = configuration.Get("PrestadorCPFCNPJ", "");
        RegistrationText.Text = configuration.Get("PrestadorIM", "");
        SchemasPathText.Text = configuration.Get("PastaSchemas", "");
        DocumentsPathText.Text = configuration.Get("CaminhoXML", Path.Combine(DataDirectory, "Documentos"));
        ExchangePathText.Text = configuration.Get("Envios", Path.Combine(DataDirectory, "Envios"));
        BatchText.Text = configuration.Get("Lote", "1");
        CompanyNameText.Text = configuration.Get("PrestadorRazaoSocial", "");
        TradeNameText.Text = configuration.Get("PrestadorFantasia", "");
        PhoneText.Text = configuration.Get("PrestadorFone", "");
        ZipCodeText.Text = configuration.Get("PrestadorCEP", "");
        StreetText.Text = configuration.Get("PrestadorEndereco", "");
        StreetNumberText.Text = configuration.Get("PrestadorNumero", "");
        ComplementText.Text = configuration.Get("PrestadorComplemento", "");
        DistrictText.Text = configuration.Get("PrestadorBairro", "");
        WebUserText.Text = configuration.Get("UsuarioWebservice", "");
        ProxyText.Text = configuration.Get("Proxy", "");
    }

    private void ConnectSettingsEvents()
    {
        EnvironmentCombo.SelectionChanged += (_, _) => { if (initialized) UpdateContext(); };
        MunicipalityInput.ValueChanged += (_, _) => { if (initialized) UpdateContext(); };
        BrowseCertificatePathButton.Click += async (_, _) => await RunAsync("Selecionar certificado", async _ =>
        {
            if (await PickFileAsync("Selecionar certificado", "Certificado PFX/P12", "*.pfx", "*.p12") is { } file)
                CertificatePathText.Text = file.TryGetLocalPath() ?? throw new InvalidOperationException("Selecione um arquivo local.");
        });
        BrowseSchemasPathButton.Click += async (_, _) => await SelectFolderAsync(SchemasPathText);
        BrowseDocumentsPathButton.Click += async (_, _) => await SelectFolderAsync(DocumentsPathText);
        BrowseExchangePathButton.Click += async (_, _) => await SelectFolderAsync(ExchangePathText);
        SaveSettingsButton.Click += async (_, _) => await RunAsync("Salvar configurações", _ => { ApplyConfiguration(); SaveConfiguration(); return Task.CompletedTask; });
    }

    private void ApplyConfiguration()
    {
        var config = client.Configuracoes;
        var code = (int)(MunicipalityInput.Value ?? 0);
        var municipality = ProviderManager.Municipios.SingleOrDefault(item => item.Codigo == code);
        if (municipality == null) throw new ArgumentException("Selecione um município cadastrado na página Municípios.");
        config.WebServices.CodigoMunicipio = code;
        config.WebServices.Ambiente = Selected<DFeTipoAmbiente>(EnvironmentCombo);
        config.WebServices.AjustaAguardaConsultaRet = true;
        config.WebServices.AguardarConsultaRet = (uint)(TimeoutInput.Value ?? 100);
        config.Certificados.Certificado = !string.IsNullOrWhiteSpace(CertificatePathText.Text) ? CertificatePathText.Text.Trim() : CertificateSerialText.Text?.Trim() ?? "";
        config.Certificados.Senha = CertificatePasswordText.Text ?? "";
        config.Arquivos.PathSchemas = string.IsNullOrWhiteSpace(SchemasPathText.Text) ? Path.Combine(AppContext.BaseDirectory, "Schemas") : SchemasPathText.Text.Trim();
        config.Arquivos.PathNFSe = DocumentsPathText.Text ?? "";
        config.Arquivos.PathRps = Path.Combine(config.Arquivos.PathNFSe, "RPS");
        config.Arquivos.PathLote = ExchangePathText.Text ?? "";
        config.Arquivos.Salvar = SaveDocumentsCheck.IsChecked == true;
        config.WebServices.Salvar = config.Geral.Salvar = SaveExchangeCheck.IsChecked == true;
        config.PrestadorPadrao.CpfCnpj = (TaxIdText.Text ?? "").OnlyNumbers();
        config.PrestadorPadrao.InscricaoMunicipal = RegistrationText.Text ?? "";
        config.PrestadorPadrao.RazaoSocial = CompanyNameText.Text ?? "";
        config.PrestadorPadrao.NomeFantasia = TradeNameText.Text ?? "";
        config.PrestadorPadrao.DadosContato.Telefone = PhoneText.Text ?? "";
        var address = config.PrestadorPadrao.Endereco;
        address.Cep = ZipCodeText.Text ?? "";
        address.Logradouro = StreetText.Text ?? "";
        address.Numero = StreetNumberText.Text ?? "";
        address.Complemento = ComplementText.Text ?? "";
        address.Bairro = DistrictText.Text ?? "";
        address.CodigoMunicipio = code;
        address.Municipio = municipality.Nome;
        address.Uf = municipality.UF.ToString();
        config.WebServices.Usuario = WebUserText.Text ?? "";
        config.WebServices.Senha = WebPasswordText.Text ?? "";
        config.WebServices.ChavePrivada = WebPrivateKeyText.Text ?? "";
        config.WebServices.ChaveAcesso = WebAccessKeyText.Text ?? "";
        config.WebServices.FraseSecreta = WebSecretText.Text ?? "";
        config.WebServices.Proxy = ProxyText.Text ?? "";
    }

    private void SaveConfiguration()
    {
        configuration.Set("Municipio", MunicipalityInput.Value ?? 0);
        configuration.Set("Ambiente", Selected<DFeTipoAmbiente>(EnvironmentCombo));
        configuration.Set("Timeout", TimeoutInput.Value ?? 100);
        configuration.Set("SalvarNfse", SaveDocumentsCheck.IsChecked == true ? "1" : "0");
        configuration.Set("SalvarEnvios", SaveExchangeCheck.IsChecked == true);
        foreach (var key in new[] { "Senha", "SenhaWebservice", "ChavePrivadaWebservice", "ChaveAcesso", "FraseSecreta" }) configuration.Set(key, "");
        configuration.Set("Certificado", CertificatePathText.Text ?? "");
        configuration.Set("NumeroSerie", CertificateSerialText.Text ?? "");
        configuration.Set("PrestadorCPFCNPJ", TaxIdText.Text ?? "");
        configuration.Set("PrestadorIM", RegistrationText.Text ?? "");
        configuration.Set("PastaSchemas", SchemasPathText.Text ?? "");
        configuration.Set("CaminhoXML", DocumentsPathText.Text ?? "");
        configuration.Set("Envios", ExchangePathText.Text ?? "");
        configuration.Set("Lote", BatchText.Text ?? "");
        configuration.Set("PrestadorRazaoSocial", CompanyNameText.Text ?? "");
        configuration.Set("PrestadorFantasia", TradeNameText.Text ?? "");
        configuration.Set("PrestadorFone", PhoneText.Text ?? "");
        configuration.Set("PrestadorCEP", ZipCodeText.Text ?? "");
        configuration.Set("PrestadorEndereco", StreetText.Text ?? "");
        configuration.Set("PrestadorNumero", StreetNumberText.Text ?? "");
        configuration.Set("PrestadorComplemento", ComplementText.Text ?? "");
        configuration.Set("PrestadorBairro", DistrictText.Text ?? "");
        configuration.Set("UsuarioWebservice", WebUserText.Text ?? "");
        configuration.Set("Proxy", ProxyText.Text ?? "");
        configuration.Save();
    }

    private void UpdateContext()
    {
        var code = (int)(MunicipalityInput.Value ?? 0);
        var service = ProviderManager.Municipios.SingleOrDefault(item => item.Codigo == code);
        var environment = Selected<DFeTipoAmbiente>(EnvironmentCombo);
        ProviderText.Text = service == null ? "Município não cadastrado" : $"{service.Provedor} / {service.Versao}";
        ContextText.Text = $"IBGE {code} · {service?.Nome} · {service?.Provedor} · {service?.Versao}";
        EnvironmentText.Text = environment == DFeTipoAmbiente.Producao ? "Produção" : "Homologação";
        EnvironmentBadge.Background = Avalonia.Media.Brush.Parse(environment == DFeTipoAmbiente.Producao ? "#FFE8DB" : "#EAF2FF");
    }

    private async Task SelectFolderAsync(TextBox target) => await RunAsync("Selecionar diretório", async _ =>
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Selecionar diretório" });
        if (folders.FirstOrDefault() is { } folder) target.Text = folder.TryGetLocalPath() ?? target.Text;
    });
    internal static void Fill<T>(ComboBox combo, IEnumerable<T> values, T selected)
    {
        combo.ItemsSource = values.ToArray();
        combo.SelectedItem = selected;
        if (combo.SelectedIndex < 0) combo.SelectedIndex = 0;
    }
    internal static T Selected<T>(ComboBox combo) => (T)combo.SelectedItem!;
}
