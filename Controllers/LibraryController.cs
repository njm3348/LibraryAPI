using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using WebApplication1.Model;
using WebApplication1.Repository;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class LibraryController
    {
        private readonly LibraryRepository libraryRepository;
        public LibraryController(IConfiguration configuration)
        {
            libraryRepository = new LibraryRepository(configuration);
        }

        //GET api/Library
        [HttpGet]
        public IEnumerable<Library> Get()
        {
            return libraryRepository.FindAll();
        }

        //GET api/Library/{id}
        [HttpGet("{id}")]
        public Library Get(int id)
        {
            return libraryRepository.FindById(id);
        }

        //POST api/Library
        [HttpPost]
        public int Post([FromBody]Library library)
        {
            return libraryRepository.Add(library);
        }

        //PUT api/Library/{id}
        [HttpPut("{id}")]
        public string Put(int id, [FromBody]Library library)
        {
            library.LibraryId = id;
            return libraryRepository.Update(library);
        }

        //DELETE api/Library/{id}
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return libraryRepository.Remove(id);
        }

        //PUT api/Library/addbook
        [HttpPut("addbook")]
        public string AddBook([FromBody]LibraryBook libraryBook)
        {
            return libraryRepository.AddBook(libraryBook);
        }

        //DELETE api/Library/removebook
        [HttpDelete("removebook")]
        public string RemoveBook([FromBody]LibraryBook libraryBook)
        {
            return libraryRepository.RemoveBook(libraryBook);
        }
    }
}
