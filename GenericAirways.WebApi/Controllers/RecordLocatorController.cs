using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using GenericAirways.Contracts;
using GenericAirways.Model;

namespace GenericAirways.WebApi.Controllers
{

    [EnableCors("AllowAllOrigins")]
    [Route("api/[controller]")]
    public class RecordLocatorController : Controller
    {

        private IRecordLocatorRepository RecordLocatorRepository;

        public RecordLocatorController(IRecordLocatorRepository recordLocatorRepository) {

            RecordLocatorRepository = recordLocatorRepository;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(RecordLocatorRepository.GetAll());
        }

        [HttpPost]
        public IActionResult Post(RecordLocator record)
        {
            RecordLocatorRepository.Add(record);
            return Ok();
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            return Ok(RecordLocatorRepository.GetAll().First(pr=>pr.Id==id));
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, RecordLocator value)
        {
            value.Id = id;
            RecordLocatorRepository.Update(value);
            return Ok();
        }
        
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            RecordLocatorRepository.Remove(new RecordLocator() { Id = id });
            return Ok();
        }

        
    }
}
