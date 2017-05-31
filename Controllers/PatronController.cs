using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using WebApplication1.Model;
using WebApplication1.Repository;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class PatronController
    {
        private readonly PatronRepository patronRepository;
        public PatronController(IConfiguration configuration)
        {
            patronRepository = new PatronRepository(configuration);
        }

        //GET api/Patron
        [HttpGet]
        public IEnumerable<Patron> Get()
        {
            return patronRepository.FindAll();
        }

        //GET api/Patron/{id}
        [HttpGet("{id}")]
        public Patron Get(int id)
        {
            return patronRepository.FindById(id);
        }

        //POST api/Patron
        [HttpPost]
        public int Post([FromBody]Patron patron)
        {
            return patronRepository.Add(patron);
        }

        //PUT api/Patron/{id}
        [HttpPut("{id}")]
        public string Put(int id, [FromBody]Patron patron)
        {
            patron.PatronId = id;
            return patronRepository.Update(patron);
        }

        //DELETE api/Patron/{id}
        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return patronRepository.Remove(id);
        }

        //PUT api/Patron/checkout
        [HttpPut("checkout")]
        public string CheckOutBook([FromBody]PatronBook patronBook)
        {
            return patronRepository.CheckOutBook(patronBook);
        }

        //DELETE api/Patron/checkin
        [HttpDelete("checkin")]
        public string CheckInBook([FromBody]PatronBook patronBook)
        {
            return patronRepository.CheckInBook(patronBook);
        }
    }
}
