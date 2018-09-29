using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<ScoreEntry[]> GetScoresFrom(int rank, int days)
        {
            var players = await _repository.GetPlayersWithBannedStatus(false);
            var scores = await _repository.GetScoresDescending(days);

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


        public async Task<ScoreEntry[]> PlayersBestScores(string name, int timeFrame)
        {
            var player = await _repository.GetPlayer(name);
            if (player == null || player.Banned)
            {
                ScoreEntry[] s = new ScoreEntry[0];
                return s;
            }
            else
            {
                return await _repository.GetPlayersScores(player.Name, timeFrame);
            }
        }

        public async Task<float> GetAverageScore()
        {
            var players = await _repository.GetPlayersWithBannedStatus(false);
            List<ScoreEntry[]> scores = new List<ScoreEntry[]>();
            foreach (Player p in players)
            {
                ScoreEntry[] sublist = await _repository.GetPlayersScores(p.Name, 0);
                scores.Add(sublist);
            }
            int summary = 0;
            int numberOfScores = 0;
            foreach (ScoreEntry[] s in scores)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    summary += s[i].Score;
                    numberOfScores++;
                }
            }
            return (float) summary / (float) numberOfScores;
            
        }

        public async Task<int> GetCommonScore()
        {
            var players = await _repository.GetPlayersWithBannedStatus(false);
            List<ScoreEntry[]> scores = new List<ScoreEntry[]>();
            foreach (Player p in players)
            {
                ScoreEntry[] sublist = await _repository.GetPlayersScores(p.Name, 0);
                scores.Add(sublist);
            }
            Dictionary <int, int> dict = new Dictionary<int, int>();
            foreach (ScoreEntry[] s in scores)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    int thisScore = s[i].Score;
                    dict.TryGetValue(thisScore, out var currentCount);
                    dict[thisScore] = currentCount + 1;
                }
            } 
            var a = dict.FirstOrDefault(x => x.Value == dict.Values.Max()).Key;
            return a;
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