using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using GenericAirways.Contracts;
using GenericAirways.Model;
using Microsoft.AspNetCore.Authorization;

namespace GenericAirways.WebApi.Controllers
{

    [EnableCors("AllowAllOrigins")]
    [Route("api/[controller]")]
    public class UserController : Controller
    {

        //private IPNLProcessor<PNLFile, PassengerRecord, string> Processor;
        private IUserRepository UserRepository;

        public UserController(IUserRepository repository) {

            UserRepository = repository;
        }
        
        [HttpPost]
        public IActionResult Post(User user)
        {
            if (ModelState.IsValid)
            {
                UserRepository.Add(user);
                return Ok();
            }
            return null;
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            return Ok(UserRepository.GetAll().First(u=>u.Id==id));
        }

        [HttpGet()]
        public IActionResult Get()
        {
            return Ok(UserRepository.GetAll());
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult Put(int id, User value)
        {
            value.Id = id;
            UserRepository.Update(value);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            UserRepository.Remove(new User() { Id = id });
            return Ok();
        }
  
        
    }
}
