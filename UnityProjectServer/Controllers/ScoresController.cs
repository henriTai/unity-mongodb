using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UnityProjectServer.Processors;
using UnityProjectServer.Models;

namespace UnityProjectServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoresController : ControllerBase
    {

        public ScoresProcessor _processor;

        public ScoresController(ScoresProcessor scores, PlayersProcessor players)
        {
            _processor = scores;
        }

        [HttpGet]
        public async Task<ScoreEntry[]> Get()
        {
            return await _processor.TopTen();
        }

        [HttpGet("{name}")]
        public ActionResult<string> Get(string name)
        {
            return "value";
        }

        [HttpPost]
        public async Task<EntryResult> Post([FromBody] NewEntry newEntry)
        {
            return await _processor.AddNewEntry(newEntry);
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
