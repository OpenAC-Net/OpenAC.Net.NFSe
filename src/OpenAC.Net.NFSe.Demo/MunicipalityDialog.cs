using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using OpenAC.Net.DFe.Core.Common;
using OpenAC.Net.NFSe.Commom;
using OpenAC.Net.NFSe.Commom.Model;
using OpenAC.Net.NFSe.Commom.Types;
using OpenAC.Net.NFSe.Providers;

namespace OpenAC.Net.NFSe.Demo;

internal sealed class MunicipalityDialog : Window
{
    private readonly Dictionary<TipoUrl, TextBox> production = new();
    private readonly Dictionary<TipoUrl, TextBox> homologation = new();
    private readonly Dictionary<string, TextBox> parameters = new();

    public MunicipalityDialog(OpenMunicipioNFSe? original)
    {
        Title = original == null ? "Novo município" : "Editar município";
        Width = 850; Height = 760; MinWidth = 650; MinHeight = 520;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        var code = new NumericUpDown { Minimum = 1, Maximum = 9999999, Value = original?.Codigo ?? 1, FormatString = "0" };
        var name = new TextBox { Text = original?.Nome ?? "" };
        var state = new ComboBox();
        MainWindow.Fill(state, Enum.GetValues<DFeSiglaUF>(), original?.UF ?? DFeSiglaUF.MS);
        var provider = new ComboBox();
        MainWindow.Fill(provider, ProviderManager.Providers.Keys.OrderBy(item => item.ToString()), original?.Provedor ?? ProviderManager.Providers.Keys.First());
        var version = new ComboBox();
        void RefreshVersions() => MainWindow.Fill(version, ProviderManager.Providers[MainWindow.Selected<NFSeProvider>(provider)].Keys,
            original?.Versao ?? VersaoNFSe.ve100);
        RefreshVersions();
        var parameterPanel = new StackPanel { Spacing = 12 };
        var values = original?.Parametros ?? new Dictionary<string, string?>();
        void RefreshParameters()
        {
            foreach (var pair in parameters) values[pair.Key] = pair.Value.Text;
            parameters.Clear(); parameterPanel.Children.Clear();
            var selected = MainWindow.Selected<NFSeProvider>(provider);
            var known = ParametrosProvider.Parametros.TryGetValue(selected, out var definitions) ? definitions : [];
            foreach (var key in values.Keys.Concat(known.Select(item => item.Nome)).Distinct())
            {
                var input = new TextBox { Text = values.GetValueOrDefault(key) ?? "" };
                parameters.Add(key, input);
                var description = known.FirstOrDefault(item => item.Nome == key)?.Descricao;
                parameterPanel.Children.Add(Field(key + (description == null ? "" : " · " + description), input));
            }
        }
        // Copy values so cancelling the editor cannot mutate the catalog.
        values = new Dictionary<string, string?>(values);
        RefreshParameters();
        provider.SelectionChanged += (_, _) => { RefreshVersions(); RefreshParameters(); };
        var fields = new StackPanel { Spacing = 12 };
        fields.Children.Add(Field("Código IBGE", code)); fields.Children.Add(Field("Município", name));
        fields.Children.Add(Field("UF", state)); fields.Children.Add(Field("Provedor", provider)); fields.Children.Add(Field("Versão", version));
        var tabs = new TabControl
        {
            Items =
            {
                new TabItem { Header = "Município", Content = new ScrollViewer { Content = fields } },
                new TabItem { Header = "Homologação", Content = UrlPanel(original?.UrlHomologacao, homologation) },
                new TabItem { Header = "Produção", Content = UrlPanel(original?.UrlProducao, production) },
                new TabItem { Header = "Parâmetros", Content = new ScrollViewer { Content = parameterPanel } }
            }
        };
        var error = new TextBlock { Foreground = Brushes.Firebrick, TextWrapping = TextWrapping.Wrap };
        var save = new Button { Content = "Salvar município", Classes = { "primary" } };
        var cancel = new Button { Content = "Voltar" };
        cancel.Click += (_, _) => Close(null);
        save.Click += (_, _) =>
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name.Text)) throw new ArgumentException("Informe o nome do município.");
                ValidateUrls(production); ValidateUrls(homologation);
                var result = new OpenMunicipioNFSe
                {
                    Codigo = (int)(code.Value ?? 0), Nome = name.Text.Trim(), UF = MainWindow.Selected<DFeSiglaUF>(state),
                    Provedor = MainWindow.Selected<NFSeProvider>(provider), Versao = MainWindow.Selected<VersaoNFSe>(version),
                    UrlProducao = production.ToDictionary(pair => pair.Key, pair => pair.Value.Text?.Trim() ?? ""),
                    UrlHomologacao = homologation.ToDictionary(pair => pair.Key, pair => pair.Value.Text?.Trim() ?? ""),
                    Parametros = parameters.Where(pair => !string.IsNullOrWhiteSpace(pair.Value.Text))
                        .ToDictionary(pair => pair.Key, pair => (string?)pair.Value.Text)
                };
                Close(result);
            }
            catch (Exception exception) { error.Text = exception.Message; }
        };
        var actions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 10 };
        actions.Children.Add(cancel); actions.Children.Add(save);
        var layout = new Grid { RowDefinitions = new RowDefinitions("*,Auto,Auto"), Margin = new Thickness(24), RowSpacing = 14 };
        layout.Children.Add(tabs); Grid.SetRow(error, 1); layout.Children.Add(error); Grid.SetRow(actions, 2); layout.Children.Add(actions);
        Content = layout;
    }

    private static Control Field(string label, Control control)
    {
        var panel = new StackPanel { Spacing = 6 };
        panel.Children.Add(new TextBlock { Text = label, TextWrapping = TextWrapping.Wrap }); panel.Children.Add(control);
        return panel;
    }
    private static ScrollViewer UrlPanel(Dictionary<TipoUrl, string>? values, Dictionary<TipoUrl, TextBox> inputs)
    {
        var panel = new StackPanel { Spacing = 12 };
        foreach (var type in Enum.GetValues<TipoUrl>())
        {
            var input = new TextBox { Text = values?.GetValueOrDefault(type) ?? "" };
            inputs.Add(type, input); panel.Children.Add(Field(type.ToString(), input));
        }
        return new ScrollViewer { Content = panel };
    }
    private static void ValidateUrls(Dictionary<TipoUrl, TextBox> inputs)
    {
        foreach (var pair in inputs)
            if (!string.IsNullOrWhiteSpace(pair.Value.Text) &&
                (!Uri.TryCreate(pair.Value.Text.Trim(), UriKind.Absolute, out var uri) || uri.Scheme is not ("https" or "http")))
                throw new ArgumentException($"Informe uma URL HTTP/HTTPS válida para {pair.Key}.");
    }
}

internal sealed class MunicipalityItem(OpenMunicipioNFSe service)
{
    public OpenMunicipioNFSe Service { get; } = service;
    public int Code => Service.Codigo;
    public string Name => Service.Nome;
    public string State => Service.UF.ToString();
    public string Provider => $"{Service.Provedor} / {Service.Versao}";
}
