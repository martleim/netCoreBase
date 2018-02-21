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
    public class PassengerListController : Controller
    {

        //private IPNLProcessor<PNLFile, PassengerRecord, string> Processor;
        private IPNLFileDataLogic PNLFileDataLogic;

        public PassengerListController(//IPNLProcessor<PNLFile, PassengerRecord, string> processor,
            IPNLFileDataLogic pNLFileDataLogic) {

            //Processor = processor;
            PNLFileDataLogic = pNLFileDataLogic;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(PNLFileDataLogic.GetAll());
        }

        [HttpPost]
        public IActionResult Post(PNLFile file)
        {
            PNLFileDataLogic.SavePnlFile(file);
            return Ok();
        }

        [HttpGet("{id:int}")]
        public IActionResult Get(int id)
        {
            return Ok(PNLFileDataLogic.GetAll().First(pnlf=>pnlf.Id==id));
        }

        [HttpPut("{id:int}")]
        public IActionResult Put(int id, PNLFile value)
        {
            value.Id = id;
            PNLFileDataLogic.Update(value);
            return Ok();
        }
        
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            PNLFileDataLogic.Remove(new PNLFile() { Id = id });
            return Ok();
        }
/* 
        [ImportFileParamType.SwaggerFormAttribute("TXT", "Upload pdf file")]
        public IHttpActionResult Post()
        {
            var files = GetFiles();
            if (files != null && files.Count > 0)
            {
                StringReader sr = new StringReader(new StreamReader(files.Get(0).InputStream).ReadToEnd());
                var processed = Processor.ProcessPNL(sr);
                PNLFileDataLogic.SavePnlFile(processed);
                processed.File = new BinaryReader(files.Get(0).InputStream).ReadBytes(files.Get(0).ContentLength);
                return Ok(processed.RecordLocator.GroupBy(rl=>rl.Code).Select(g=> new {
                    Code = g.Key,
                    PassengerRecords = g.SelectMany(g2=>g2.PassengerRecord)
                }));
            }
            return Ok();
        }

        private HttpFileCollection GetFiles()
        {

            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count < 1)
            {
                return null;
            }

            return httpRequest.Files;
        }*/
        
    }
}
