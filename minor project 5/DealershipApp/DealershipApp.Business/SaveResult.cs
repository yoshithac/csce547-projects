namespace DealershipApp.Business;

/// <summary>What happened when a Save was requested.</summary>
public enum SaveStatus
{
    /// <summary>A new row was added (the object's ID was 0).</summary>
    Inserted,

    /// <summary>An existing row was changed.</summary>
    Updated,

    /// <summary>The object had an ID, but no row with that ID exists.</summary>
    NotFound,

    /// <summary>The object broke a business rule, so nothing was saved.</summary>
    Invalid
}

/// <summary>
/// The answer from a business-layer Save method: the outcome, the row's ID,
/// and any validation messages grouped by field name.
/// </summary>
public class SaveResult
{
    private SaveResult(SaveStatus status, int id, IReadOnlyDictionary<string, string[]> errors)
    {
        Status = status;
        Id = id;
        Errors = errors;
    }

    public SaveStatus Status { get; }

    /// <summary>The ID of the saved row (the new ID after an insert).</summary>
    public int Id { get; }

    /// <summary>Validation messages keyed by field name. Empty unless Status is Invalid.</summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public bool Succeeded => Status is SaveStatus.Inserted or SaveStatus.Updated;

    private static readonly IReadOnlyDictionary<string, string[]> NoErrors = new Dictionary<string, string[]>();

    public static SaveResult Inserted(int id) => new(SaveStatus.Inserted, id, NoErrors);

    public static SaveResult Updated(int id) => new(SaveStatus.Updated, id, NoErrors);

    public static SaveResult NotFound(int id) => new(SaveStatus.NotFound, id, NoErrors);

    public static SaveResult Invalid(IReadOnlyDictionary<string, string[]> errors) => new(SaveStatus.Invalid, 0, errors);
}
