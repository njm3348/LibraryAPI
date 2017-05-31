using System.Collections.Generic;

namespace WebApplication1.Model
{
    public class Patron
    {
        public int PatronId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public List<Book> Books { get; set; }
    }
}
