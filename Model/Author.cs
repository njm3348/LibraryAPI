using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public List<Book> Books { get; set; }

    }
}
