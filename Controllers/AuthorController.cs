using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApplication1.Repository;
using WebApplication1.Model;
using Microsoft.Extensions.Configuration;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class AuthorController
    {
        private readonly AuthorRepository authorRepository;
        public AuthorController(IConfiguration configuration)
        {
            authorRepository = new AuthorRepository(configuration);
        }

        //GET api/Author
        [HttpGet]
        public IEnumerable<Author> Get()
        {
            return authorRepository.FindAll();
        }

        //GET api/Author/{id}
        [HttpGet("{id}")]
        public Author Get(int id)
        {
            return authorRepository.FindByID(id);
        }

        //POST api/Author
        [HttpPost]
        public int Post([FromBody]Author author)
        {
            return authorRepository.Add(author);
        }

        //PUT api/Author/{id}
        [HttpPut("{id}")]
        public string Put(int id, [FromBody]Author author)
        {
            author.AuthorId = id;
            return authorRepository.Update(author);
        }

        //DELETE api/Author/{id}
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return authorRepository.Remove(id);
        }

        //PUT api/Author/addbook
        [HttpPut("addbook")]
        public string AddBook([FromBody]BookAuthor bookAuthor)
        {
            return authorRepository.AddBook(bookAuthor);
        }

        //DELETE api/Author/removebook
        [HttpDelete("removebook")]
        public string RemoveBook([FromBody]BookAuthor bookAuthor)
        {
            return authorRepository.RemoveBook(bookAuthor);
        }
    }
}
