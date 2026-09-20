using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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
						Console.WriteLine("Error: that conflicts with a foreign key (a book needs a valid author, and an author with books can't be deleted).");
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

		// ---------- The 4 menu options ----------

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
				Console.WriteLine("Author added:");
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
				Console.WriteLine("Book added:");
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
				Console.WriteLine(rows + " row(s)