namespace BookstoreConsole
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; } = "";
        public int AuthorID { get; set; }
        public string Genre { get; set; } = "";
        public int? PublishedYear { get; set; }
        public decimal Price { get; set; }
    }
}