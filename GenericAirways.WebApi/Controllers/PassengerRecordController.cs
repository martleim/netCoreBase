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
    public class PassengerRecordController : Controller
    {

        private IPassengerRecordRepository PassengerRecordRepository;

        public PassengerRecordController(IPassengerRecordRepository passengerRecordRepository) {

            PassengerRecordRepository = passengerRecordRepository;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(PassengerRecordRepository.GetAll());
        }

        [HttpPost]
        public IActionResult Post(PassengerRecord record)
        {
            record.RecordLocator = null;

            PassengerRecordRepository.Add(record);
            return Ok();
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            return Ok(PassengerRecordRepository.GetAll().First(pr=>pr.Id==id));
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, PassengerRecord value)
        {
            value.Id = id;
            PassengerRecordRepository.Update(value);
            return Ok();
        }
        
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            PassengerRecordRepository.Remove(new PassengerRecord() { Id = id });
            return Ok();
        }

        
    }
}
