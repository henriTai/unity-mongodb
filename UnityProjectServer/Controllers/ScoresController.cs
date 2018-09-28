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
        [HttpGet("{name}")]
        public async Task<ScoreEntry[]> Get(int? slice, string name)
        {
            if (slice.HasValue)
            {
                if (slice > 1 && slice < 5000)
                {
                    return await _processor.GetScoresFrom((int)slice);
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                return await _processor.PlayersBestScores(name);
            }
            return await _processor.GetScoresFrom(0);
        }


        [HttpPost]
        public async Task<EntryResult> Post([FromBody] NewEntry newEntry)
        {
            return await _processor.AddNewEntry(newEntry);
        }

        [HttpPut("{id}")]
        public string Put(int id, [FromBody] string value)
        {
            return "Sorry mate, no cheese.";
        }

        [HttpDelete("{id}")]
        public string Delete(int id)
        {
            return "Can't do that, sorry.";
        }
    }
}
