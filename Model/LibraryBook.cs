namespace WebApplication1.Model
{
    public class LibraryBook
    {
        public int LibraryBookId { get; set; }
        public int LibraryId { get; set; }
        public int BookId { get; set; }
        public bool CheckedOut { get; set; }
    }
}
