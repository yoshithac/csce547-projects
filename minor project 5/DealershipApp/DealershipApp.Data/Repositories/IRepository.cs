namespace DealershipApp.Data.Repositories;

/// <summary>
/// The five CRUD operations every table supports.
/// </summary>
/// <typeparam name="T">The data object for the table.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>SELECT every row.</summary>
    Task<List<T>> GetAllAsync();

    /// <summary>SELECT one row by primary key, or null if it doesn't exist.</summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>INSERT a row and return the new identity value.</summary>
    Task<int> InsertAsync(T item);

    /// <summary>UPDATE a row. Returns false if no row has that primary key.</summary>
    Task<bool> UpdateAsync(T item);

    /// <summary>DELETE a row. Returns false if no row has that primary key.</summary>
    Task<bool> DeleteAsync(int id);
}
