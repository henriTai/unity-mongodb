using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UnityProjectServer.Models;
using UnityProjectServer.Processors;

namespace UnityProjectServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        public PlayersProcessor _processor;

        public PlayersController (PlayersProcessor processor)
        {
            _processor = processor;
        }

        [HttpGet]
        public async Task<Player[]> GetAll(bool? banned)
        {
            if (banned.HasValue)
            {
                return await _processor.GetAllWithBannedStatus((bool) banned);
            }
            return await _processor.GetAll();
        }
        [HttpGet("{id:Guid}")]
        [HttpGet("{name}")]
        public async Task<Player> Get(Guid id, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return await _processor.GetWithName(name);
            }
            return await _processor.GetWithID(id);
        }

        [HttpPost]
        public string Post()
        {
            return "This function is not in use.";
        }

        [HttpPut("{id:Guid}")]
        [HttpPut("{name}")]
        public async Task<Player> Put(Guid id, string name, bool? banned)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (banned.HasValue)
                {
                    return await _processor.SetBannedStatusWithName (name, (bool) banned);
                }
                else
                {
                    return await _processor.ChangeBannedStatusWithName (name);
                }
            }
            else
            {
                if (banned.HasValue)
                {
                   return await  _processor.SetBannedStatusWithID (id, (bool) banned);
                }
                else
                {
                   return await  _processor.ChangeBannedStatusWithID (id);
                }
            }
        }

        [HttpDelete("{id:Guid}")]
        [HttpDelete("{name}")]
        public async Task<string> Delete(Guid id, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return await _processor.DeletePlayer(name);
            }
            else
            {
                return await _processor.DeletePlayerWithID(id);
            }
        }
    }
}