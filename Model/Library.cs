using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class Library
    {
        public int LibraryId { get; set; }
        public string Name { get; set; }
        public List<Book> Books { get; set; }
    }
}
