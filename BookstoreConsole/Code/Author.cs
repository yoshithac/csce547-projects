namespace BookstoreConsole
{
    public class Author
    {
        public int AuthorID { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Country { get; set; } = "";
        public int? BirthYear { get; set; }
    }
}