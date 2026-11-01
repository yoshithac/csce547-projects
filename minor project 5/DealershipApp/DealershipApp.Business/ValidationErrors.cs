namespace DealershipApp.Business;

/// <summary>
/// Collects validation messages while a Save method checks its rules.
/// </summary>
internal sealed class ValidationErrors
{
    private readonly Dictionary<string, List<string>> _errors = new();

    public bool Any => _errors.Count > 0;

    public void Add(string field, string message)
    {
        if (!_errors.TryGetValue(field, out var messages))
        {
            messages = new List<string>();
            _errors[field] = messages;
        }

        messages.Add(message);
    }

    /// <summary>Checks that a text value is present and fits its database column.</summary>
    public void RequireText(string field, string value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Add(field, $"{field} is required.");
        }
        else if (value.Length > maxLength)
        {
            Add(field, $"{field} must be {maxLength} characters or fewer.");
        }
    }

    public IReadOnlyDictionary<string, string[]> ToDictionary() =>
        _errors.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray());
}
