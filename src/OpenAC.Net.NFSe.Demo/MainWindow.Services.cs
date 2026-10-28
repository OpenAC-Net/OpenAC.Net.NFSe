using Avalonia.Platform.Storage;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Demo;

public partial class MainWindow
{
    private void ConnectServicesEvents()
    {
        ServiceSearchText.TextChanged += (_, _) => RefreshServices();
        NewServiceButton.Click += async (_, _) => await RunAsync("Cadastrar município", token => EditServiceAsync(null, token));
        EditServiceButton.Click += async (_, _) => await RunAsync("Editar município", token => EditServiceAsync(SelectedService(), token));
        CopyServiceButton.Click += async (_, _) => await RunAsync("Copiar município", token => EditServiceAsync(SelectedService(), token, true));
        DeleteServiceButton.Click += async (_, _) => await RunAsync("Excluir município", async token =>
        {
            var service = SelectedService();
            var answer = await InputDialog.AskAsync(this, "Excluir município", new InputField("codigo", $"Digite {service.Codigo} para excluir {service.Nome}"));
            if (answer == null) return;
            if (answer["codigo"] != service.Codigo.ToString()) throw new ArgumentException("O código informado não corresponde ao município.");
            token.ThrowIfCancellationRequested();
            PersistCatalog(ProviderManager.Municipios.Where(item => item != service).ToArray());
            RefreshServices(); UpdateContext();
        });
        ServiceList.DoubleTapped += async (_, _) => { if (ServiceList.SelectedItem is MunicipalityItem item) await RunAsync("Editar município", token => EditServiceAsync(item.Service, token)); };
        UseServiceButton.Click += async (_, _) => await RunAsync("Selecionar município", _ =>
        {
            MunicipalityInput.Value = SelectedService().Codigo; ApplyConfiguration(); SaveConfiguration(); UpdateContext();
            return Task.CompletedTask;
        });
        LoadServicesButton.Click += async (_, _) => await RunAsync("Importar catálogo", async token =>
        {
            var file = await PickFileAsync("Importar catálogo de municípios", "Catálogo NFS-e", "*.nfse", "*.xml");
            if (file == null) return;
            await using var stream = await file.OpenReadAsync();
            var catalog = ProviderManager.ReadCatalog(stream);
            ValidateImportedCatalog(catalog);
            token.ThrowIfCancellationRequested();
            PersistCatalog(catalog); RefreshServices(); UpdateContext();
            ResultText.Text = $"Catálogo importado: {catalog.Length} municípios.";
        });
        SaveServicesButton.Click += async (_, _) => await RunAsync("Exportar catálogo", async token =>
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Exportar municípios", SuggestedFileName = "Municipios.nfse", DefaultExtension = "nfse",
                FileTypeChoices = [new FilePickerFileType("Catálogo NFS-e") { Patterns = ["*.nfse"] }]
            });
            if (file == null) return;
            token.ThrowIfCancellationRequested();
            await using var stream = await file.OpenWriteAsync(); stream.SetLength(0); ProviderManager.Save(stream);
        });
    }
    private OpenMunicipioNFSe SelectedService() => (ServiceList.SelectedItem as MunicipalityItem)?.Service
        ?? throw new ArgumentException("Selecione um município na lista.");
    private void RefreshServices()
    {
        var code = (ServiceList.SelectedItem as MunicipalityItem)?.Code;
        var search = ServiceSearchText.Text?.Trim() ?? "";
        var items = ProviderManager.Municipios.OrderBy(service => service.Nome).Select(service => new MunicipalityItem(service))
            .Where(item => $"{item.Code} {item.Name} {item.State} {item.Provider}".Contains(search, StringComparison.OrdinalIgnoreCase)).ToArray();
        ServiceList.ItemsSource = items; ServiceList.SelectedItem = items.FirstOrDefault(item => item.Code == code);
    }
    private async Task EditServiceAsync(OpenMunicipioNFSe? original, CancellationToken token, bool copy = false)
    {
        var result = await new MunicipalityDialog(original) { Icon = Icon }.ShowDialog<OpenMunicipioNFSe?>(this);
        if (result == null) return;
        token.ThrowIfCancellationRequested();
        var updated = ProviderManager.Municipios.Where(item => copy || item != original).ToList(); updated.Add(result);
        ValidateImportedCatalog(updated.ToArray()); PersistCatalog(updated.ToArray()); RefreshServices(); UpdateContext();
        ServiceList.SelectedItem = ((IEnumerable<MunicipalityItem>)ServiceList.ItemsSource!).FirstOrDefault(item => item.Code == result.Codigo);
        ResultText.Text = $"Município salvo: {result.Nome}.";
    }
    private void PersistCatalog(OpenMunicipioNFSe[] municipalities)
    {
        using var serialized = new MemoryStream(); ProviderManager.WriteCatalog(serialized, municipalities);
        var temporary = catalogPath + ".tmp";
        try { File.WriteAllBytes(temporary, serialized.ToArray()); File.Move(temporary, catalogPath, true); }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
        serialized.Position = 0; ProviderManager.Load(serialized);
    }
    private static void ValidateImportedCatalog(OpenMunicipioNFSe[] municipalities)
    {
        if (municipalities.Length == 0) throw new ArgumentException("O catálogo não contém municípios.");
        if (municipalities.GroupBy(item => item.Codigo).Any(group => group.Count() > 1)) throw new ArgumentException("O catálogo contém códigos IBGE duplicados. Use um código diferente ao copiar.");
        if (municipalities.Any(item => item.Codigo < 1 || string.IsNullOrWhiteSpace(item.Nome))) throw new ArgumentException("Informe nome e código IBGE válido para todos os municípios.");
    }
}
