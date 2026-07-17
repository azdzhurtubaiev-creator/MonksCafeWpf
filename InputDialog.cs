using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MonksCafeWpf;

public static class InputDialog
{
    public static string Show(string question, string title)
    {
        var dlg = new Window
        {
            Title = title, Width = 400, Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Background = Brushes.Black, ResizeMode = ResizeMode.NoResize
        };
        var panel = new StackPanel { Margin = new Thickness(15) };
        panel.Children.Add(new TextBlock
        {
            Text = question, Foreground = Brushes.White, Margin = new Thickness(0, 0, 0, 10)
        });
        var input = new TextBox { FontSize = 14, Padding = new Thickness(5) };
        panel.Children.Add(input);
        var btn = new Button
        {
            Content = "OK", Margin = new Thickness(0, 10, 0, 0), Padding = new Thickness(10, 5, 10, 5),
            Background = Brushes.DarkOrange, Foreground = Brushes.White
        };
        string result = "";
        btn.Click += (s, e) => { result = input.Text; dlg.Close(); };
        input.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Enter) { result = input.Text; dlg.Close(); } };
        panel.Children.Add(btn);
        dlg.Content = panel;
        input.Focus();
        dlg.ShowDialog();
        return result;
    }
}