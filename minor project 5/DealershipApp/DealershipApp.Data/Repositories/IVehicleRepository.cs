using DealershipApp.Data.Entities;

namespace DealershipApp.Data.Repositories;

/// <summary>CRUD for dbo.Vehicle, plus a lookup by dealership.</summary>
public interface IVehicleRepository : IRepository<Vehicle>
{
    /// <summary>SELECT every vehicle on one dealership's lot.</summary>
    Task<List<Vehicle>> GetByDealershipIdAsync(int dealershipId);
}
