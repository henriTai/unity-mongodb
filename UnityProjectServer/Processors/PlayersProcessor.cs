using System;
using System.Threading.Tasks;
using UnityProjectServer.Models;
using UnityProjectServer.Repositories;

namespace UnityProjectServer.Processors
{
    public class PlayersProcessor
    {
        IRepository _repository;

        public PlayersProcessor(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Player[]> GetAll()
        {
            return await _repository.GetAllPlayers();
        }

        public async Task<Player[]> GetAllWithBannedStatus(bool banned)
        {
           return await _repository.GetPlayersWithBannedStatus(banned);
        }


        public async Task<Player> SetBannedStatus(Player player)
        {
            return await _repository.SetBannedStatus(player);
        }

        public async Task<Player> GetWithName(string name)
        {
            return await _repository.GetPlayer(name);
        }

        public async Task<Player> GetWithID(Guid id)
        {
            return await _repository.GetPlayerWithID (id);
        }

        public async Task<Player> SetBannedStatusWithName(string name, bool banned)
        {
            return await _repository.SetBannedStatusWithName(name, banned);
        }

        public async Task<Player> ChangeBannedStatusWithName(string name)
        {
            return await _repository.ChangeBannedStatusWithName(name);
        }

        public async Task<Player> SetBannedStatusWithID(Guid id, bool banned)
        {
            return await _repository.SetBannedStatusWithID(id, banned);
        }

        public async Task<Player> ChangeBannedStatusWithID(Guid id)
        {
            return await _repository.ChangeBannedStatusWithID(id);
        }
    }
}