using System.Xml.Linq;

namespace QBReconcile.Services;

public static class ExtensionUtilities
{
    public static bool IsNullOrEmpty(this string? value) => string.IsNullOrEmpty(value);

    public static int AsInt(this XAttribute? attribute, int defaultValue = 0) => int.TryParse(attribute?.Value, out int value) ? value : defaultValue;
    public static string? AsString(this XAttribute? attribute) => attribute?.Value;
    public static DateTime? AsDateTime(this XAttribute? attribute) => DateTime.TryParse(attribute?.Value, out var value) ? value : null;
    public static decimal? AsDecimal(this XAttribute? attribute) => decimal.TryParse(attribute?.Value, out var value) ? value : null;
}
