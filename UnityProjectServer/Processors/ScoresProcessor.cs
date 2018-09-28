using System;
using System.Collections.Generic;
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

        public async Task<ScoreEntry[]> GetScoresFrom(int rank)
        {
            var players = await _repository.GetPlayersWithBannedStatus(false);
            var scores = await _repository.GetScoresDescending();

            List<ScoreEntry> entries = new List<ScoreEntry>();
            if (scores.Length > 0)
            {
                int inList = 0;
                for (int i = 0; i < scores.Length; i++)
                { 
                    for (int j = 0; j < players.Length; j++)
                    {
                        if (scores[i].Name.Equals(players[j].Name))
                        {
                            if (rank > 1)
                            {
                                rank--;
                            }
                            else
                            {
                                entries.Add(scores[i]);
                                inList++;
                                break; 
                            }
                        }
                    }
                    if (inList==10) break;
                }               
            }
            return entries.ToArray();
        }

        public async Task<ScoreEntry[]> PlayersBestScores(string name)
        {
            var player = await _repository.GetPlayer(name);
            if (player == null || player.Banned)
            {
                ScoreEntry[] s = new ScoreEntry[0];
                return s;
            }
            else
            {
                return await _repository.GetPlayersScores(player.Name);
            }
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