using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace BookstoreConsole
{
    public class DataLayer
    {
        private string connectionString =
            @"Server=localhost\SQLEXPRESS;Database=BookstoreDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // ---------- Database info ----------

        public List<string> GetTableNames()
        {
            List<string> tables = new List<string>();
            string sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES " +
                         "WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    tables.Add(reader["TABLE_NAME"].ToString());
                }
                reader.Close();
            }
            return tables;
        }

        public int GetRecordCount(string tableName)
        {
            string sql;
            if (tableName == "Authors")
                sql = "SELECT COUNT(*) FROM Authors";
            else if (tableName == "Books")
                sql = "SELECT COUNT(*) FROM Books";
            else
                return 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public List<string> GetColumnDescriptions(string tableName)
        {
            List<string> columns = new List<string>();
            string sql = "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS " +
                         "WHERE TABLE_NAME = @tableName ORDER BY ORDINAL_POSITION";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@tableName", tableName);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string nullText = reader["IS_NULLABLE"].ToString() == "YES" ? "NULL" : "NOT NULL";
                    columns.Add(reader["COLUMN_NAME"] + " - " + reader["DATA_TYPE"] + " - " + nullText);
                }
                reader.Close();
            }
            return columns;
        }

        // ---------- Authors ----------

        public List<Author> GetAllAuthors()
        {
            List<Author> authors = new List<Author>();
            string sql = "SELECT AuthorID, FirstName, LastName, Country, BirthYear FROM Authors ORDER BY AuthorID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    authors.Add(ReadAuthor(reader));
                }
                reader.Close();
            }
            return authors;
        }

        public Author GetAuthorById(int id)
        {
            Author author = null;
            string sql = "SELECT AuthorID, FirstName, LastName, Country, BirthYear FROM Authors WHERE AuthorID = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    author = ReadAuthor(reader);
                }
                reader.Close();
            }
            return author;
        }

        public int InsertAuthor(Author author)
        {
            string sql = "INSERT INTO Authors (FirstName, LastName, Country, BirthYear) " +
                         "VALUES (@firstName, @lastName, @country, @birthYear); " +
                         "SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@firstName", author.FirstName);
                command.Parameters.AddWithValue("@lastName", author.LastName);
                command.Parameters.AddWithValue("@country", DbValue(author.Country));
                command.Parameters.AddWithValue("@birthYear", DbValue(author.BirthYear));
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int UpdateAuthor(int id, string columnName, string newValue)
        {
            string[] allowedColumns = { "FirstName", "LastName", "Country", "BirthYear" };
            if (Array.IndexOf(allowedColumns, columnName) < 0)
                return 0;

            string sql = "UPDATE Authors SET " + columnName + " = @value WHERE AuthorID = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@value", DbValue(newValue));
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public int DeleteAuthor(int id)
        {
            string sql = "DELETE FROM Authors WHERE AuthorID = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        // ---------- Books ----------

        public List<Book> GetAllBooks()
        {
            List<Book> books = new List<Book>();
            string sql = "SELECT BookID, Title, AuthorID, Genre, PublishedYear, Price FROM Books ORDER BY BookID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    books.Add(ReadBook(reader));
                }
                reader.Close();
            }
            return books;
        }

        public Book GetBookById(int id)
        {
            Book book = null;
            string sql = "SELECT BookID, Title, AuthorID, Genre, PublishedYear, Price FROM Books WHERE BookID = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    book = ReadBook(reader);
                }
                reader.Close();
            }
            return book;
        }

        public int InsertBook(Book book)
        {
            string sql = "INSERT INTO Books (Title, AuthorID, Genre, PublishedYear, Price) " +
                         "VALUES (@title, @authorId, @genre, @publishedYear, @price); " +
                         "SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@title", book.Title);
                command.Parameters.AddWithValue("@authorId", book.AuthorID);
                command.Parameters.AddWithValue("@genre", DbValue(book.Genre));
                command.Parameters.AddWithValue("@publishedYear", DbValue(book.PublishedYear));
                command.Parameters.AddWithValue("@price", book.Price);
                connection.Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public int UpdateBook(int id, string columnName, string newValue)
        {
            string[] allowedColumns = { "Title", "AuthorID", "Genre", "PublishedYear", "Price" };
            if (Array.IndexOf(allowedColumns, columnName) < 0)
                return 0;

            string sql = "UPDATE Books SET " + columnName + " = @value WHERE BookID = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@value", DbValue(newValue));
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        public int DeleteBook(int id)
        {
            string sql = "DELETE FROM Books WHERE BookID = @id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }

        // ---------- Helpers ----------

        private Author ReadAuthor(SqlDataReader reader)
        {
            Author author = new Author();
            author.AuthorID = Convert.ToInt32(reader["AuthorID"]);
            author.FirstName = reader["FirstName"].ToString();
            author.LastName = reader["LastName"].ToString();
            author.Country = reader["Country"].ToString();
            if (reader["BirthYear"] != DBNull.Value)
                author.BirthYear = Convert.ToInt32(reader["BirthYear"]);
            return author;
        }

        private Book ReadBook(SqlDataReader reader)
        {
            Book book = new Book();
            book.BookID = Convert.ToInt32(reader["BookID"]);
            book.Title = reader["Title"].ToString();
            book.AuthorID = Convert.ToInt32(reader["AuthorID"]);
            book.Genre = reader["Genre"].ToString();
            if (reader["PublishedYear"] != DBNull.Value)
                book.PublishedYear = Convert.ToInt32(reader["PublishedYear"]);
            book.Price = Convert.ToDecimal(reader["Price"]);
            return book;
        }

        // Turns null or empty values into a database NULL
        private object DbValue(object value)
        {
            if (value == null || value.ToString() == "")
                return DBNull.Value;
            return value;
        }
    }
}
