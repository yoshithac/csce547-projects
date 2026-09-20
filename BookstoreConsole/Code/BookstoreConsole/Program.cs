using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace BookstoreConsole
{
	class Program
	{
		static DataLayer data = new DataLayer();

		static void Main(string[] args)
		{
			try
			{
				ShowSummary();
			}
			catch (SqlException ex)
			{
				Console.WriteLine("Could not connect to the database.");
				Console.WriteLine(ex.Message);
				Console.ReadLine();
				return;
			}

			bool running = true;
			while (running)
			{
				Console.WriteLine();
				Console.WriteLine("===== MENU =====");
				Console.WriteLine("1. Get data");
				Console.WriteLine("2. Add data");
				Console.WriteLine("3. Update data");
				Console.WriteLine("4. Delete data");
				Console.WriteLine("5. Quit");
				Console.Write("Choose an option: ");
				string choice = Console.ReadLine();

				try
				{
					if (choice == "1")
						GetData();
					else if (choice == "2")
						AddData();
					else if (choice == "3")
						UpdateData();
					else if (choice == "4")
						DeleteData();
					else if (choice == "5")
						running = false;
					else
						Console.WriteLine("Invalid choice. Please enter 1-5.");
				}
				catch (SqlException ex)
				{
					if (ex.Number == 547)
						Console.WriteLine("Error: That conflicts with a foreign key constraint (a book needs a valid author, and an author with existing books cannot be deleted).");
					else
						Console.WriteLine("Database error: " + ex.Message);
				}
			}

			Console.WriteLine("Goodbye!");
		}

		// ---------- Summary ----------

		static void ShowSummary()
		{
			Console.WriteLine("===== DATABASE SUMMARY =====");
			List<string> tables = data.GetTableNames();
			Console.WriteLine("Number of tables: " + tables.Count);

			foreach (string table in tables)
			{
				Console.WriteLine();
				Console.WriteLine("Table: " + table + " (" + data.GetRecordCount(table) + " records)");
				foreach (string column in data.GetColumnDescriptions(table))
				{
					Console.WriteLine("   " + column);
				}
			}

			Console.WriteLine();
			Console.WriteLine("Books.AuthorID is a foreign key to Authors.AuthorID");
		}

		// ---------- The 4 Menu Options ----------

		static void GetData()
		{
			int table = PickTable();
			if (table == 1)
				ShowAuthors(data.GetAllAuthors());
			else if (table == 2)
				ShowBooks(data.GetAllBooks());
		}

		static void AddData()
		{
			int table = PickTable();
			if (table == 1)
			{
				Author author = new Author();
				author.FirstName = ReadRequired("First name");
				author.LastName = ReadRequired("Last name");
				author.Country = ReadOptional("Country (Enter to skip)");
				author.BirthYear = ReadOptionalInt("Birth year (Enter to skip)");

				int newId = data.InsertAuthor(author);
				Console.WriteLine("Author added successfully:");
				ShowAuthors(new List<Author> { data.GetAuthorById(newId) });
			}
			else if (table == 2)
			{
				Console.WriteLine("Existing authors:");
				ShowAuthors(data.GetAllAuthors());

				Book book = new Book();
				book.Title = ReadRequired("Title");
				book.AuthorID = ReadInt("Author ID (from the list above)");
				book.Genre = ReadOptional("Genre (Enter to skip)");
				book.PublishedYear = ReadOptionalInt("Published year (Enter to skip)");
				book.Price = ReadDecimal("Price");

				int newId = data.InsertBook(book);
				Console.WriteLine("Book added successfully:");
				ShowBooks(new List<Book> { data.GetBookById(newId) });
			}
		}

		static void UpdateData()
		{
			int table = PickTable();
			if (table == 1)
			{
				ShowAuthors(data.GetAllAuthors());
				int id = ReadInt("Enter the AuthorID to update");
				if (data.GetAuthorById(id) == null)
				{
					Console.WriteLine("No author with that ID.");
					return;
				}

				string[] columns = { "FirstName", "LastName", "Country", "BirthYear" };
				string column = PickColumn(columns);
				if (column == null)
					return;

				Console.Write("Enter the new value for " + column + ": ");
				string newValue = Console.ReadLine();

				int rows = data.UpdateAuthor(id, column, newValue);
				Console.WriteLine(rows + " row(s) updated. Updated record:");
				ShowAuthors(new List<Author> { data.GetAuthorById(id) });
			}
			else if (table == 2)
			{
				ShowBooks(data.GetAllBooks());
				int id = ReadInt("Enter the BookID to update");
				if (data.GetBookById(id) == null)
				{
					Console.WriteLine("No book with that ID.");
					return;
				}

				string[] columns = { "Title", "AuthorID", "Genre", "PublishedYear", "Price" };
				string column = PickColumn(columns);
				if (column == null)
					return;

				Console.Write("Enter the new value for " + column + ": ");
				string newValue = Console.ReadLine();

				int rows = data.UpdateBook(id, column, newValue);
				Console.WriteLine(rows + " row(s) updated. Updated record:");
				ShowBooks(new List<Book> { data.GetBookById(id) });
			}
		}

		static void DeleteData()
		{
			int table = PickTable();
			if (table == 1)
			{
				ShowAuthors(data.GetAllAuthors());
				int id = ReadInt("Enter the AuthorID to delete");
				if (data.GetAuthorById(id) == null)
				{
					Console.WriteLine("No author with that ID.");
					return;
				}

				int rows = data.DeleteAuthor(id);
				Console.WriteLine(rows + " row(s) deleted.");
				ShowAuthors(data.GetAllAuthors());
			}
			else if (table == 2)
			{
				ShowBooks(data.GetAllBooks());
				int id = ReadInt("Enter the BookID to delete");
				if (data.GetBookById(id) == null)
				{
					Console.WriteLine("No book with that ID.");
					return;
				}

				int rows = data.DeleteBook(id);
				Console.WriteLine(rows + " row(s) deleted.");
				ShowBooks(data.GetAllBooks());
			}
		}

		// ---------- UI Helpers ----------

		static int PickTable()
		{
			while (true)
			{
				Console.WriteLine("\nSelect Table:");
				Console.WriteLine("1. Authors");
				Console.WriteLine("2. Books");
				Console.Write("Choice: ");
				string input = Console.ReadLine();
				if (input == "1") return 1;
				if (input == "2") return 2;
				Console.WriteLine("Invalid choice. Enter 1 or 2.");
			}
		}

		static string PickColumn(string[] columns)
		{
			Console.WriteLine("Columns available to update: " + string.Join(", ", columns));
			Console.Write("Enter column name exactly as shown: ");
			string choice = Console.ReadLine()?.Trim();
			foreach (var col in columns)
			{
				if (col.Equals(choice, StringComparison.OrdinalIgnoreCase))
					return col;
			}
			Console.WriteLine("Invalid column name.");
			return null;
		}

		static void ShowAuthors(List<Author> authors)
		{
			Console.WriteLine($"\n{"ID",-5} | {"First Name",-15} | {"Last Name",-15} | {"Country",-18} | {"Birth Year",-10}");
			Console.WriteLine(new string('-', 72));
			foreach (var a in authors)
			{
				Console.WriteLine($"{a.AuthorID,-5} | {a.FirstName,-15} | {a.LastName,-15} | {a.Country,-18} | {a.BirthYear?.ToString() ?? "NULL",-10}");
			}
			Console.WriteLine();
		}

		static void ShowBooks(List<Book> books)
		{
			Console.WriteLine($"\n{"ID",-5} | {"Title",-32} | {"AuthorID",-9} | {"Genre",-18} | {"Year",-6} | {"Price",-8}");
			Console.WriteLine(new string('-', 85));
			foreach (var b in books)
			{
				Console.WriteLine($"{b.BookID,-5} | {b.Title,-32} | {b.AuthorID,-9} | {b.Genre,-18} | {b.PublishedYear?.ToString() ?? "NULL",-6} | ${b.Price,-8:F2}");
			}
			Console.WriteLine();
		}

		static string ReadRequired(string label)
		{
			while (true)
			{
				Console.Write($"{label}: ");
				string input = Console.ReadLine()?.Trim();
				if (!string.IsNullOrEmpty(input))
					return input;
				Console.WriteLine("This field is required.");
			}
		}

		static string ReadOptional(string label)
		{
			Console.Write($"{label}: ");
			return Console.ReadLine()?.Trim() ?? "";
		}

		static int ReadInt(string label)
		{
			while (true)
			{
				Console.Write($"{label}: ");
				if (int.TryParse(Console.ReadLine(), out int val))
					return val;
				Console.WriteLine("Please enter a valid whole number.");
			}
		}

		static int? ReadOptionalInt(string label)
		{
			Console.Write($"{label}: ");
			string input = Console.ReadLine()?.Trim();
			if (string.IsNullOrEmpty(input))
				return null;
			if (int.TryParse(input, out int val))
				return val;
			Console.WriteLine("Invalid number format. Skipping value.");
			return null;
		}

		static decimal ReadDecimal(string label)
		{
			while (true)
			{
				Console.Write($"{label}: ");
				if (decimal.TryParse(Console.ReadLine(), out decimal val))
					return val;
				Console.WriteLine("Please enter a valid decimal number.");
			}
		}
	}
}