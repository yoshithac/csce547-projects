using System.Data;
using System.Globalization;

namespace BookstoreConsole;

public static class ConsoleHelper
{
    public static string Prompt(string message)
    {
        Console.Write($"{message}: ");
        return (Console.ReadLine() ?? "").Trim();
    }

    public static int? PromptInt(string message)
    {
        string input = Prompt(message);
        if (int.TryParse(input, out int value))
            return value;

        Console.WriteLine("Please enter a whole number.");
        return null;
    }

    public static void Header(string text)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 60));
        Console.WriteLine($" {text}");
        Console.WriteLine(new string('=', 60));
    }

    public static void PrintTable(DataTable data)
    {
        Console.WriteLine();
        if (data.Rows.Count == 0)
        {
            Console.WriteLine("  (no records)");
            return;
        }

        const int MaxWidth = 40;
        int colCount = data.Columns.Count;

        var cells = new string[data.Rows.Count][];
        for (int r = 0; r < data.Rows.Count; r++)
        {
            cells[r] = new string[colCount];
            for (int c = 0; c < colCount; c++)
                cells[r][c] = FormatValue(data.Rows[r][c]);
        }

        var widths = new int[colCount];
        for (int c = 0; c < colCount; c++)
        {
            int widest = data.Columns[c].ColumnName.Length;
            foreach (var row in cells)
                widest = Math.Max(widest, row[c].Length);
            widths[c] = Math.Min(widest, MaxWidth);
        }

        var headers = data.Columns.Cast<DataColumn>().Select((col, i) => Fit(col.ColumnName, widths[i]));
        Console.WriteLine(string.Join(" | ", headers));
        Console.WriteLine(string.Join("-+-", widths.Select(w => new string('-', w))));

        foreach (var row in cells)
            Console.WriteLine(string.Join(" | ", row.Select((text, i) => Fit(text, widths[i]))));

        Console.WriteLine($"({data.Rows.Count} record(s))");
    }

    private static string FormatValue(object value)
    {
        if (value is DBNull) return "NULL";
        if (value is DateTime d) return d.ToString("yyyy-MM-dd");
        return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
    }

    private static string Fit(string text, int width)
    {
        if (text.Length > width)
            return text.Substring(0, width - 3) + "...";
        return text.PadRight(width);
    }
}