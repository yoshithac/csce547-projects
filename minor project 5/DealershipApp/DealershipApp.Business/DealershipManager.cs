using DealershipApp.Data.Entities;
using DealershipApp.Data.Repositories;

namespace DealershipApp.Business;

/// <summary>
/// Business methods for dealerships. Callers never choose between Insert and Update:
/// they call SaveAsync, which inserts when the ID is 0 and updates otherwise.
/// </summary>
public class DealershipManager
{
    private readonly IDealershipRepository _dealerships;

    public DealershipManager(IDealershipRepository dealerships)
    {
        _dealerships = dealerships;
    }

    public Task<List<Dealership>> GetAllAsync() => _dealerships.GetAllAsync();

    public Task<Dealership?> GetByIdAsync(int id) => _dealerships.GetByIdAsync(id);

    /// <summary>
    /// Returns a DealershipId → Name lookup in one query, so other screens can show
    /// "Apex Motors" instead of "Dealership 1".
    /// </summary>
    public async Task<Dictionary<int, string>> GetNameLookupAsync()
    {
        var dealerships = await _dealerships.GetAllAsync();
        return dealerships.ToDictionary(d => d.DealershipId, d => d.Name);
    }

    /// <summary>
    /// Inserts the dealership when DealershipId is 0, otherwise updates it.
    /// Text is trimmed and checked against the column sizes before anything is saved.
    /// </summary>
    public async Task<SaveResult> SaveAsync(Dealership dealership)
    {
        dealership.Name = dealership.Name?.Trim() ?? string.Empty;
        dealership.City = dealership.City?.Trim() ?? string.Empty;

        var errors = new ValidationErrors();
        if (dealership.DealershipId < 0)
        {
            errors.Add(nameof(Dealership.DealershipId), "DealershipId cannot be negative. Use 0 to add a new dealership.");
        }

        errors.RequireText(nameof(Dealership.Name), dealership.Name, 100);
        errors.RequireText(nameof(Dealership.City), dealership.City, 50);

        if (errors.Any)
        {
            return SaveResult.Invalid(errors.ToDictionary());
        }

        if (dealership.DealershipId == 0)
        {
            var newId = await _dealerships.InsertAsync(dealership);
            return SaveResult.Inserted(newId);
        }

        var updated = await _dealerships.UpdateAsync(dealership);
        return updated
            ? SaveResult.Updated(dealership.DealershipId)
            : SaveResult.NotFound(dealership.DealershipId);
    }

    /// <summary>
    /// Deletes a dealership. The database cascades the delete to its salespeople and vehicles.
    /// Returns false if the dealership doesn't exist.
    /// </summary>
    public Task<bool> DeleteAsync(int id) => _dealerships.DeleteAsync(id);
}
