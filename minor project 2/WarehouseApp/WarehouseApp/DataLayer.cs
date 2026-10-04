using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WarehouseApp
{
    public class DataLayer
    {
        private readonly string _connectionString =
            @"Server=localhost\SQLEXPRESS;Database=WarehouseDB;Trusted_Connection=True;TrustServerCertificate=True;";

        private SqlConnection GetConnection() => new SqlConnection(_connectionString);

        public DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(query, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public int ExecuteNonQuery(string query, SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddRange(parameters);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // ==================== 1. SUPPLIERS (ADMIN) ====================
        public DataTable GetAllSuppliers() => ExecuteQuery("SELECT * FROM Suppliers ORDER BY SupplierID");

        public int InsertSupplier(string name, string email, string phone, string city) =>
            ExecuteNonQuery("INSERT INTO Suppliers (SupplierName, ContactEmail, Phone, City) VALUES (@n, @e, @p, @c)",
                new[] {
                    new SqlParameter("@n", name),
                    new SqlParameter("@e", email),
                    new SqlParameter("@p", phone),
                    new SqlParameter("@c", city)
                });

        public int UpdateSupplier(int id, string name, string email, string phone, string city) =>
            ExecuteNonQuery("UPDATE Suppliers SET SupplierName=@n, ContactEmail=@e, Phone=@p, City=@c WHERE SupplierID=@id",
                new[] {
                    new SqlParameter("@id", id),
                    new SqlParameter("@n", name),
                    new SqlParameter("@e", email),
                    new SqlParameter("@p", phone),
                    new SqlParameter("@c", city)
                });

        public int DeleteSupplier(int id) =>
            ExecuteNonQuery("DELETE FROM Suppliers WHERE SupplierID=@id",
                new[] { new SqlParameter("@id", id) });

        // ==================== 2. PRODUCTS (SEARCH & CRUD) ====================
        public DataTable GetAllProducts() => ExecuteQuery("SELECT * FROM Products ORDER BY ProductID");

        public DataTable SearchProducts(string keyword) =>
            ExecuteQuery(@"SELECT * FROM Products 
                           WHERE ProductName LIKE @k 
                              OR SKU LIKE @k 
                              OR Category LIKE @k 
                           ORDER BY ProductID",
                new[] { new SqlParameter("@k", $"%{keyword}%") });

        public int InsertProduct(string sku, string name, string cat, decimal price, int stock, int suppId) =>
            ExecuteNonQuery(@"INSERT INTO Products (SKU, ProductName, Category, UnitPrice, StockQuantity, SupplierID) 
                             VALUES (@sku, @name, @cat, @price, @stock, @suppId)",
                new[] {
                    new SqlParameter("@sku", sku),
                    new SqlParameter("@name", name),
                    new SqlParameter("@cat", cat),
                    new SqlParameter("@price", price),
                    new SqlParameter("@stock", stock),
                    new SqlParameter("@suppId", suppId)
                });

        public int UpdateProduct(int id, string sku, string name, string cat, decimal price, int stock, int suppId) =>
            ExecuteNonQuery(@"UPDATE Products 
                             SET SKU=@sku, ProductName=@name, Category=@cat, UnitPrice=@price, StockQuantity=@stock, SupplierID=@suppId 
                             WHERE ProductID=@id",
                new[] {
                    new SqlParameter("@id", id),
                    new SqlParameter("@sku", sku),
                    new SqlParameter("@name", name),
                    new SqlParameter("@cat", cat),
                    new SqlParameter("@price", price),
                    new SqlParameter("@stock", stock),
                    new SqlParameter("@suppId", suppId)
                });

        public int DeleteProduct(int id) =>
            ExecuteNonQuery("DELETE FROM Products WHERE ProductID=@id",
                new[] { new SqlParameter("@id", id) });

        // ==================== LOOKUPS (for ComboBoxes) ====================
        public DataTable GetSupplierList() =>
            ExecuteQuery("SELECT SupplierID, SupplierName FROM Suppliers ORDER BY SupplierName");

        public DataTable GetProductList() =>
            ExecuteQuery("SELECT ProductID, SKU + ' - ' + ProductName AS Display, UnitPrice FROM Products ORDER BY ProductName");

        // ==================== 3. ORDERS (CRUD) ====================
        public DataTable GetAllOrders() => ExecuteQuery("SELECT * FROM Orders ORDER BY OrderID");

        public int InsertOrder(string customer, int productId, int qty, decimal total) =>
            ExecuteNonQuery(@"INSERT INTO Orders (CustomerName, OrderDate, ProductID, Quantity, TotalAmount) 
                             VALUES (@c, GETDATE(), @pid, @qty, @tot)",
                new[] {
                    new SqlParameter("@c", customer),
                    new SqlParameter("@pid", productId),
                    new SqlParameter("@qty", qty),
                    new SqlParameter("@tot", total)
                });

        public int UpdateOrder(int id, string customer, int productId, int qty, decimal total) =>
            ExecuteNonQuery(@"UPDATE Orders 
                             SET CustomerName=@c, ProductID=@pid, Quantity=@qty, TotalAmount=@tot 
                             WHERE OrderID=@id",
                new[] {
                    new SqlParameter("@id", id),
                    new SqlParameter("@c", customer),
                    new SqlParameter("@pid", productId),
                    new SqlParameter("@qty", qty),
                    new SqlParameter("@tot", total)
                });

        public int DeleteOrder(int id) =>
            ExecuteNonQuery("DELETE FROM Orders WHERE OrderID=@id",
                new[] { new SqlParameter("@id", id) });
    }
}