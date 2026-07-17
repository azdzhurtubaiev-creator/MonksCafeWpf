using System.Globalization;

namespace MonksCafeWpf;

public static class Validators
{
    public static bool IsValidDescription(string desc)
    {
        return !string.IsNullOrEmpty(desc) && desc.Length >= 3 && desc.Length <= 20;
    }

    public static bool TryParsePrice(string input, out decimal price)
    {
        price = 0m;
        if (string.IsNullOrEmpty(input)) return false;
        input = input.Replace(',', '.');
        return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out price) && price > 0;
    }

    public static bool IsValidFileName(string name)
    {
        return !string.IsNullOrEmpty(name) && name.Length >= 1 && name.Length <= 10;
    }
}