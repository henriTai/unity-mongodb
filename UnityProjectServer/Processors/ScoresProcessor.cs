using System;
using System.Threading.Tasks;
using UnityProjectServer.Models;
using UnityProjectServer.Repositories;

namespace UnityProjectServer.Processors
{
    public class ScoresProcessor
    {
        IRepository _repository;

        public ScoresProcessor(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<ScoreEntry[]> TopTen()
        {
            return await _repository.TopTen();
        }

        public async Task<EntryResult> AddNewEntry(NewEntry newEntry)
        {
            await _repository.AddNewPlayerIfNecessary(newEntry.Name);
            bool banned = await _repository.IsPlayerBanned(newEntry.Name);
            if (banned)
            {
                EntryResult result = new EntryResult();
                result.Name = newEntry.Name;
                result.Banned = true;
                result.BestRanking = 0;
                result.BestScore = 0;
                result.Score = 0;
                result.Ranking = 0;
                return result;
            }
            else
            {
                ScoreEntry entry = new ScoreEntry();
                entry.Date = DateTime.UtcNow;
                entry.Name = newEntry.Name;
                entry.Score = newEntry.Score;
                entry._id = Guid.NewGuid();

                return await _repository.AddNewEntry(entry);
            }
        }
    }
}