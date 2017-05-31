using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model
{
    public class Book
    { 
        public int BookId { get; set; }
        public string Title { get; set; }
        public List<Author> Authors { get; set; }
        public List<Library> Libraries { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
