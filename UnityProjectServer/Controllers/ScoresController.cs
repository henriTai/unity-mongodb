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
        public async Task<ScoreEntry[]> Get(int? slice, string name, int? days)
        {
            if (slice.HasValue)
            {
                if (slice > 1 && slice < 5000)
                {
                    if (days.HasValue && (int) days > 0)
                    {
                        return await _processor.GetScoresFrom((int)slice, (int) days);
                    }
                    return await _processor.GetScoresFrom((int)slice, 0);
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                if (days.HasValue && (int) days > 0)
                {
                    return await _processor.PlayersBestScores(name, (int)days);
                }
                return await _processor.PlayersBestScores(name, 0);
            }
            if (days.HasValue)
            {
                return await _processor.GetScoresFrom(0, (int) days);
            }
            return await _processor.GetScoresFrom(0, 0);
        }

        [HttpGet("common")]
        public async Task<int> GetCommonScore()
        {
            return await _processor.GetCommonScore();
        }

        [HttpGet("average")]
        public async Task<float> GetAverageScore()
        {
            return await _processor.GetAverageScore();
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
