using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using WebApplication1.Model;
using WebApplication1.Repository;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class BookController
    {
        private readonly BookRepository bookRepository;

        public BookController(IConfiguration configuration)
        {
            bookRepository = new BookRepository(configuration);
        }

        //GET api/Book
        [HttpGet]
        public IEnumerable<Book> Get()
        {
            return bookRepository.FindAll();
        }

        //GET api/Book/{id}
        [HttpGet("{id}")]
        public Book Get(int id)
        {
            return bookRepository.FindByID(id);
        }

        //POST api/Book
        [HttpPost]
        public int Post([FromBody]Book book)
        {
            return bookRepository.Add(book);
        }

        //PUT api/Book/{id}
        [HttpPut("{id}")]
        public string Put(int id, [FromBody]Book book)
        {
            book.BookId = id;
            return bookRepository.Update(book);
        }

        //DELETE api/Book/{id}
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return bookRepository.Remove(id);
        }

        //PUT api/Book/addauthor
        [HttpPut("addauthor")]
        public string AddAuthor([FromBody]BookAuthor bookAuthor)
        {
            return bookRepository.AddAuthor(bookAuthor);
        }

        //DELETE api/Book/removeauthor
        [HttpDelete("removeauthor")]
        public string RemoveAuthor([FromBody]BookAuthor bookAuthor)
        {
            return bookRepository.RemoveAuthor(bookAuthor);
        }

    }
}
