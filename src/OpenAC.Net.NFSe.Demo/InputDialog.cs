using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;

namespace OpenAC.Net.NFSe.Demo;

internal sealed record InputField(string Key, string Label, string Value = "", bool Required = true);

internal sealed class InputDialog : Window
{
    private InputDialog(string title, IReadOnlyList<InputField> fields)
    {
        Title = title;
        Width = 510;
        MaxHeight = 780;
        CanResize = false;
        SizeToContent = SizeToContent.Height;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        var content = new StackPanel { Spacing = 14, Margin = new Thickness(24) };
        content.Children.Add(new TextBlock { Text = title, FontSize = 20, FontWeight = Avalonia.Media.FontWeight.SemiBold });
        var inputs = new Dictionary<string, TextBox>();
        foreach (var field in fields)
        {
            var panel = new StackPanel { Spacing = 5 };
            panel.Children.Add(new TextBlock { Text = field.Label });
            var input = new TextBox { Text = field.Value };
            inputs.Add(field.Key, input);
            panel.Children.Add(input);
            content.Children.Add(panel);
        }
        var validation = new TextBlock { Foreground = Avalonia.Media.Brushes.Firebrick, TextWrapping = Avalonia.Media.TextWrapping.Wrap };
        content.Children.Add(validation);
        var actions = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 10 };
        var cancel = new Button { Content = "Voltar" };
        var confirm = new Button { Content = "Continuar", Classes = { "primary" } };
        cancel.Click += (_, _) => Close(null);
        confirm.Click += (_, _) =>
        {
            var missing = fields.FirstOrDefault(field => field.Required && string.IsNullOrWhiteSpace(inputs[field.Key].Text));
            if (missing != null)
            {
                validation.Text = "Preencha: " + missing.Label;
                inputs[missing.Key].Focus();
                return;
            }
            Close(inputs.ToDictionary(pair => pair.Key, pair => pair.Value.Text?.Trim() ?? ""));
        };
        KeyDown += (_, args) => { if (args.Key == Key.Escape) Close(null); };
        actions.Children.Add(cancel);
        actions.Children.Add(confirm);
        content.Children.Add(actions);
        Content = new ScrollViewer { Content = content };
    }

    public static Task<Dictionary<string, string>?> AskAsync(Window owner, string title, params InputField[] fields)
        => new InputDialog(title, fields) { Icon = owner.Icon }.ShowDialog<Dictionary<string, string>?>(owner);
}
