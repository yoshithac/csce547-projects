using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MusicStore.Data;

// ===================== Models =====================
public class Artist
{
    public int ArtistId { get; set; }
    [Required, StringLength(100)] public string Name { get; set; }
    [Required, StringLength(50)] public string Country { get; set; }
    [Display(Name = "Active Since"), Range(1900, 2100)] public int ActiveSince { get; set; } = 2000;
}

public class Album
{
    public int AlbumId { get; set; }
    [Required, StringLength(150)] public string Title { get; set; }
    [Required, StringLength(50)] public string Genre { get; set; }
    [Display(Name = "Release Year"), Range(1900, 2100)] public int ReleaseYear { get; set; } = 2024;
    [Range(0, 9999.99)] public decimal Price { get; set; }
    [Range(0, 100000)] public int Stock { get; set; }
    [Display(Name = "Artist"), Range(1, int.MaxValue, ErrorMessage = "Please choose an artist.")] public int ArtistId { get; set; }
    public string ArtistName { get; set; }   // filled by the join, not edited
}

public class Customer
{
    public int CustomerId { get; set; }
    [Display(Name = "First Name"), Required, StringLength(50)] public string FirstName { get; set; }
    [Display(Name = "Last Name"), Required, StringLength(50)] public string LastName { get; set; }
    [Required, EmailAddress, StringLength(100)] public string Email { get; set; }
    [StringLength(20)] public string Phone { get; set; }
    [StringLength(50)] public string City { get; set; }
}

// ===================== Database helper =====================
public class Database
{
    private readonly string _cs;
    public Database(string connectionString) => _cs = connectionString;

    public List<T> Query<T>(string sql, Func<SqlDataReader, T> map, params SqlParameter[] p)
    {
        using var conn = new SqlConnection(_cs);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddRange(p);
        conn.Open();
        using var r = cmd.ExecuteReader();
        var list = new List<T>();
        while (r.Read()) list.Add(map(r));
        return list;
    }

    public int Execute(string sql, params SqlParameter[] p)
    {
        using var conn = new SqlConnection(_cs);
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddRange(p);
        conn.Open();
        return cmd.ExecuteNonQuery();
    }

    public static SqlParameter P(string name, object value) => new SqlParameter(name, value ?? DBNull.Value);
    public static string Str(SqlDataReader r, int i) => r.IsDBNull(i) ? null : r.GetString(i);
}

// ===================== Repositories =====================
public class ArtistRepo
{
    private readonly Database _db;
    public ArtistRepo(Database db) => _db = db;
    private const string Cols = "ArtistId, Name, Country, ActiveSince";
    private static Artist Map(SqlDataReader r) => new() { ArtistId = r.GetInt32(0), Name = r.GetString(1), Country = r.GetString(2), ActiveSince = r.GetInt32(3) };

    public List<Artist> GetAll() => _db.Query($"SELECT {Cols} FROM Artists ORDER BY Name", Map);
    public Artist Get(int id) => _db.Query($"SELECT {Cols} FROM Artists WHERE ArtistId=@id", Map, Database.P("@id", id)).FirstOrDefault();
    public void Add(Artist a) => _db.Execute("INSERT INTO Artists (Name, Country, ActiveSince) VALUES (@n,@c,@y)",
        Database.P("@n", a.Name), Database.P("@c", a.Country), Database.P("@y", a.ActiveSince));
    public void Update(Artist a) => _db.Execute("UPDATE Artists SET Name=@n, Country=@c, ActiveSince=@y WHERE ArtistId=@id",
        Database.P("@n", a.Name), Database.P("@c", a.Country), Database.P("@y", a.ActiveSince), Database.P("@id", a.ArtistId));
    public void Delete(int id) => _db.Execute("DELETE FROM Artists WHERE ArtistId=@id", Database.P("@id", id));
}

public class AlbumRepo
{
    private readonly Database _db;
    public AlbumRepo(Database db) => _db = db;
    private const string Select = @"SELECT a.AlbumId, a.Title, a.Genre, a.ReleaseYear, a.Price, a.Stock, a.ArtistId, ar.Name AS ArtistName
                                    FROM Albums a JOIN Artists ar ON ar.ArtistId = a.ArtistId";
    private static Album Map(SqlDataReader r) => new()
    {
        AlbumId = r.GetInt32(0), Title = r.GetString(1), Genre = r.GetString(2), ReleaseYear = r.GetInt32(3),
        Price = r.GetDecimal(4), Stock = r.GetInt32(5), ArtistId = r.GetInt32(6), ArtistName = r.GetString(7)
    };

    // Search by Title, Genre, or Artist name using LIKE (empty term returns everything)
    public List<Album> Search(string term)
    {
        string like = string.IsNullOrWhiteSpace(term) ? null : "%" + term.Trim() + "%";
        var p = new SqlParameter("@t", SqlDbType.NVarChar, 200) { Value = (object)like ?? DBNull.Value };
        return _db.Query(Select + " WHERE (@t IS NULL OR a.Title LIKE @t OR a.Genre LIKE @t OR ar.Name LIKE @t) ORDER BY a.Title", Map, p);
    }
    public Album Get(int id) => _db.Query(Select + " WHERE a.AlbumId=@id", Map, Database.P("@id", id)).FirstOrDefault();
    public void Add(Album a) => _db.Execute(
        "INSERT INTO Albums (Title, Genre, ReleaseYear, Price, Stock, ArtistId) VALUES (@t,@g,@y,@p,@s,@a)",
        Database.P("@t", a.Title), Database.P("@g", a.Genre), Database.P("@y", a.ReleaseYear),
        Database.P("@p", a.Price), Database.P("@s", a.Stock), Database.P("@a", a.ArtistId));
    public void Update(Album a) => _db.Execute(
        "UPDATE Albums SET Title=@t, Genre=@g, ReleaseYear=@y, Price=@p, Stock=@s, ArtistId=@a WHERE AlbumId=@id",
        Database.P("@t", a.Title), Database.P("@g", a.Genre), Database.P("@y", a.ReleaseYear),
        Database.P("@p", a.Price), Database.P("@s", a.Stock), Database.P("@a", a.ArtistId), Database.P("@id", a.AlbumId));
    public void Delete(int id) => _db.Execute("DELETE FROM Albums WHERE AlbumId=@id", Database.P("@id", id));
}

public class CustomerRepo
{
    private readonly Database _db;
    public CustomerRepo(Database db) => _db = db;
    private const string Cols = "CustomerId, FirstName, LastName, Email, Phone, City";
    private static Customer Map(SqlDataReader r) => new()
    {
        CustomerId = r.GetInt32(0), FirstName = r.GetString(1), LastName = r.GetString(2),
        Email = r.GetString(3), Phone = Database.Str(r, 4), City = Database.Str(r, 5)
    };

    public List<Customer> GetAll() => _db.Query($"SELECT {Cols} FROM Customers ORDER BY LastName, FirstName", Map);
    public Customer Get(int id) => _db.Query($"SELECT {Cols} FROM Customers WHERE CustomerId=@id", Map, Database.P("@id", id)).FirstOrDefault();
    public void Add(Customer c) => _db.Execute("INSERT INTO Customers (FirstName, LastName, Email, Phone, City) VALUES (@f,@l,@e,@p,@c)",
        Database.P("@f", c.FirstName), Database.P("@l", c.LastName), Database.P("@e", c.Email), Database.P("@p", c.Phone), Database.P("@c", c.City));
    public void Update(Customer c) => _db.Execute("UPDATE Customers SET FirstName=@f, LastName=@l, Email=@e, Phone=@p, City=@c WHERE CustomerId=@id",
        Database.P("@f", c.FirstName), Database.P("@l", c.LastName), Database.P("@e", c.Email), Database.P("@p", c.Phone), Database.P("@c", c.City), Database.P("@id", c.CustomerId));
    public void Delete(int id) => _db.Execute("DELETE FROM Customers WHERE CustomerId=@id", Database.P("@id", id));
}
