using System;
using System.Globalization;
using System.Windows;

namespace MonksCafeWpf;

public partial class MainWindow : Window
{
    private readonly BillService bill = new BillService();

    public MainWindow()
    {
        InitializeComponent();
        RefreshUi();
    }

    private void RefreshUi()
    {
        ItemsGrid.ItemsSource = null;
        ItemsGrid.ItemsSource = bill.Items;
        LblNet.Text = bill.GetNetTotal().ToString("F2", CultureInfo.InvariantCulture);
        LblTip.Text = bill.TipAmount.ToString("F2", CultureInfo.InvariantCulture);
        LblGst.Text = bill.GetGstAmount().ToString("F2", CultureInfo.InvariantCulture);
        LblTotal.Text = bill.GetTotalAmount().ToString("F2", CultureInfo.InvariantCulture);
    }

    private void BtnAddItem_Click(object sender, RoutedEventArgs e)
    {
        string desc = Prompt("Введіть опис (3-20 символів):", "Add Item");
        if (string.IsNullOrEmpty(desc)) return;
        if (desc.Length < 3 || desc.Length > 20)
        {
            MessageBox.Show("Опис має бути від 3 до 20 символів.", "Помилка");
            return;
        }
        string priceStr = Prompt("Введіть ціну (більше 0):", "Add Item");
        if (string.IsNullOrEmpty(priceStr)) return;
        priceStr = priceStr.Replace(',', '.');
        if (!decimal.TryParse(priceStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price) || price <= 0)
        {
            MessageBox.Show("Ціна має бути додатнім числом.", "Помилка");
            return;
        }
        if (!bill.AddItem(desc, price))
        {
            MessageBox.Show($"Максимум {BillService.MaxItems} позицій.", "Помилка");
            return;
        }
        RefreshUi();
    }

    private void BtnRemoveItem_Click(object sender, RoutedEventArgs e)
    {
        if (bill.Items.Count == 0)
        {
            MessageBox.Show("Список порожній.", "Remove Item");
            return;
        }
        string numStr = Prompt($"Введіть номер позиції (1-{bill.Items.Count}), 0 — скасувати:", "Remove Item");
        if (string.IsNullOrEmpty(numStr)) return;
        if (!int.TryParse(numStr, out int num))
        {
            MessageBox.Show("Введіть число.", "Помилка");
            return;
        }
        if (num == 0) return;
        if (num < 1 || num > bill.Items.Count)
        {
            MessageBox.Show("Номер поза діапазоном.", "Помилка");
            return;
        }
        bill.RemoveItemAt(num - 1);
        RefreshUi();
    }

    private void BtnAddTip_Click(object sender, RoutedEventArgs e)
    {
        string choice = Prompt("Оберіть тип чайових: 1 — відсоток, 2 — сума, 0 — без чайових", "Add Tip");
        if (string.IsNullOrEmpty(choice)) return;
        if (choice == "0") { bill.SetTipAmount(0m); RefreshUi(); return; }
        if (choice == "1")
        {
            string pStr = Prompt("Введіть відсоток (0-100):", "Add Tip");
            if (string.IsNullOrEmpty(pStr)) return;
            pStr = pStr.Replace(',', '.');
            if (!decimal.TryParse(pStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal p) || p < 0 || p > 100)
            {
                MessageBox.Show("Відсоток від 0 до 100.", "Помилка");
                return;
            }
            bill.SetTipPercentage(p);
            RefreshUi();
        }
        else if (choice == "2")
        {
            string aStr = Prompt("Введіть суму чайових:", "Add Tip");
            if (string.IsNullOrEmpty(aStr)) return;
            aStr = aStr.Replace(',', '.');
            if (!decimal.TryParse(aStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal a) || a < 0)
            {
                MessageBox.Show("Сума має бути невідʼємною.", "Помилка");
                return;
            }
            bill.SetTipAmount(a);
            RefreshUi();
        }
    }

    private void BtnDisplayBill_Click(object sender, RoutedEventArgs e)
    {
        if (bill.Items.Count == 0)
        {
            MessageBox.Show("Рахунок порожній.", "Display Bill");
            return;
        }
        string msg = $"Net Total: {bill.GetNetTotal():F2}\nTip: {bill.TipAmount:F2}\nGST (5%): {bill.GetGstAmount():F2}\nTOTAL: {bill.GetTotalAmount():F2}";
        MessageBox.Show(msg, "Display Bill");
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e)
    {
        bill.Clear();
        RefreshUi();
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        string name = Prompt("Введіть імʼя файлу (1-10 символів):", "Save to File");
        if (string.IsNullOrEmpty(name)) return;
        if (name.Length < 1 || name.Length > 10)
        {
            MessageBox.Show("Імʼя файлу має бути від 1 до 10 символів.", "Помилка");
            return;
        }
        if (bill.SaveToFile(name + ".csv"))
            MessageBox.Show($"Збережено у {name}.csv", "Save to File");
        else
            MessageBox.Show("Помилка збереження.", "Помилка");
    }

    private void BtnLoad_Click(object sender, RoutedEventArgs e)
    {
        string name = Prompt("Введіть імʼя файлу для завантаження:", "Load from File");
        if (string.IsNullOrEmpty(name)) return;
        if (bill.LoadFromFile(name + ".csv"))
        {
            RefreshUi();
            MessageBox.Show($"Завантажено з {name}.csv", "Load from File");
        }
        else
        {
            MessageBox.Show("Файл не знайдено.", "Помилка");
        }
    }

    private void BtnExit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private static string Prompt(string question, string title)
    {
        var dlg = new Window
        {
            Title = title, Width = 400, Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Background = System.Windows.Media.Brushes.Black, ResizeMode = ResizeMode.NoResize
        };
        var panel = new System.Windows.Controls.StackPanel { Margin = new Thickness(15) };
        panel.Children.Add(new System.Windows.Controls.TextBlock
        {
            Text = question, Foreground = System.Windows.Media.Brushes.White, Margin = new Thickness(0, 0, 0, 10)
        });
        var input = new System.Windows.Controls.TextBox { FontSize = 14, Padding = new Thickness(5) };
        panel.Children.Add(input);
        var btn = new System.Windows.Controls.Button
        {
            Content = "OK", Margin = new Thickness(0, 10, 0, 0), Padding = new Thickness(10, 5, 10, 5),
            Background = System.Windows.Media.Brushes.DarkOrange, Foreground = System.Windows.Media.Brushes.White
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