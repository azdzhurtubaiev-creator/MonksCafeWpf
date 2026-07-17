using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MonksCafeWpf;
public class BillItem
{
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
}

public class BillService
{
    public const int MaxItems = 5;
    public const decimal GstRate = 0.05m;

    private readonly List<BillItem> items = new List<BillItem>();
    private decimal tipAmount = 0m;

    public IReadOnlyList<BillItem> Items => items;
    public decimal TipAmount => tipAmount;

    public decimal GetNetTotal()
    {
        decimal total = 0m;
        foreach (BillItem item in items)
        {
            total += item.Price;
        }
        return total;
    }

    public decimal GetGstAmount() => GetNetTotal() * GstRate;

    public decimal GetTotalAmount() => GetNetTotal() + tipAmount + GetGstAmount();

    public bool AddItem(string description, decimal price)
    {
        if (items.Count >= MaxItems)
        {
            return false;
        }
        items.Add(new BillItem { Description = description, Price = price });
        return true;
    }

    public void RemoveItemAt(int index)
    {
        if (index >= 0 && index < items.Count)
        {
            items.RemoveAt(index);
        }
    }

    public void SetTipAmount(decimal amount)
    {
        tipAmount = amount;
    }

    public void SetTipPercentage(decimal percent)
    {
        tipAmount = Math.Round(GetNetTotal() * percent / 100m, 2);
    }

    public void Clear()
    {
        items.Clear();
        tipAmount = 0m;
    }

    public bool SaveToFile(string path)
    {
        try
        {
            List<string> lines = new List<string>();
            foreach (BillItem item in items)
            {
                lines.Add($"{item.Description};{item.Price.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }
            lines.Add($"TIP;{tipAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            File.WriteAllLines(path, lines);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool LoadFromFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                return false;
            }

            string[] lines = File.ReadAllLines(path);
            List<BillItem> loadedItems = new List<BillItem>();
            decimal loadedTip = 0m;

            foreach (string line in lines)
            {
                string[] parts = line.Split(';');
                if (parts.Length != 2)
                {
                    continue;
                }

                if (!decimal.TryParse(parts[1], System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal value))
                {
                    continue;
                }

                if (parts[0] == "TIP")
                {
                    loadedTip = value;
                }
                else if (loadedItems.Count < MaxItems)
                {
                    loadedItems.Add(new BillItem { Description = parts[0], Price = value });
                }
            }

            items.Clear();
            items.AddRange(loadedItems);
            tipAmount = loadedTip;
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}