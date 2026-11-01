using DealershipApp.Data.Entities;

namespace DealershipApp.Data.Repositories;

/// <summary>CRUD for dbo.Salesperson, plus a lookup by dealership.</summary>
public interface ISalespersonRepository : IRepository<Salesperson>
{
    /// <summary>SELECT every salesperson who works at one dealership.</summary>
    Task<List<Salesperson>> GetByDealershipIdAsync(int dealershipId);
}
