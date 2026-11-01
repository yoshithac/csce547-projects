using System.Net.Mail;
using DealershipApp.Data.Entities;
using DealershipApp.Data.Repositories;

namespace DealershipApp.Business;

/// <summary>
/// Business methods for salespeople: one Save for insert and update,
/// email clean-up and checks, and a check that the dealership exists.
/// </summary>
public class SalespersonManager
{
    private readonly ISalespersonRepository _salespeople;
    private readonly IDealershipRepository _dealerships;

    public SalespersonManager(ISalespersonRepository salespeople, IDealershipRepository dealerships)
    {
        _salespeople = salespeople;
        _dealerships = dealerships;
    }

    public Task<List<Salesperson>> GetAllAsync() => _salespeople.GetAllAsync();

    public Task<Salesperson?> GetByIdAsync(int id) => _salespeople.GetByIdAsync(id);

    /// <summary>Everyone who works at one dealership.</summary>
    public Task<List<Salesperson>> GetByDealershipAsync(int dealershipId) =>
        _salespeople.GetByDealershipIdAsync(dealershipId);

    /// <summary>
    /// Inserts the salesperson when SalespersonId is 0, otherwise updates them.
    /// The email is trimmed and lower-cased so the same address is always stored the same way.
    /// </summary>
    public async Task<SaveResult> SaveAsync(Salesperson salesperson)
    {
        salesperson.FullName = salesperson.FullName?.Trim() ?? string.Empty;
        salesperson.Email = salesperson.Email?.Trim().ToLowerInvariant() ?? string.Empty;

        var errors = new ValidationErrors();
        if (salesperson.SalespersonId < 0)
        {
            errors.Add(nameof(Salesperson.SalespersonId), "SalespersonId cannot be negative. Use 0 to add a new salesperson.");
        }

        errors.RequireText(nameof(Salesperson.FullName), salesperson.FullName, 100);
        errors.RequireText(nameof(Salesperson.Email), salesperson.Email, 100);

        if (salesperson.Email.Length > 0 && !IsValidEmail(salesperson.Email))
        {
            errors.Add(nameof(Salesperson.Email), "Email must be a valid email address, like name@dealer.com.");
        }

        if (await _dealerships.GetByIdAsync(salesperson.DealershipId) is null)
        {
            errors.Add(nameof(Salesperson.DealershipId), $"Dealership {salesperson.DealershipId} does not exist.");
        }

        if (errors.Any)
        {
            return SaveResult.Invalid(errors.ToDictionary());
        }

        if (salesperson.SalespersonId == 0)
        {
            var newId = await _salespeople.InsertAsync(salesperson);
            return SaveResult.Inserted(newId);
        }

        var updated = await _salespeople.UpdateAsync(salesperson);
        return updated
            ? SaveResult.Updated(salesperson.SalespersonId)
            : SaveResult.NotFound(salesperson.SalespersonId);
    }

    /// <summary>Deletes a salesperson. Returns false if they don't exist.</summary>
    public Task<bool> DeleteAsync(int id) => _salespeople.DeleteAsync(id);

    private static bool IsValidEmail(string email) =>
        MailAddress.TryCreate(email, out var parsed)
        && parsed.Address == email
        && parsed.Host.Contains('.');
}
