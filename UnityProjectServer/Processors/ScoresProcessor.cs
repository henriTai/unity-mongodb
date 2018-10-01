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

                await _repository.AddNewEntry(entry);

                var notBanned = await _repository.GetPlayersWithBannedStatus(false);
                var allScores = await _repository.GetScoresDescending(0);

                List<ScoreEntry> scores = new List<ScoreEntry>();

                for (int i = 0; i < allScores.Length; i++)
                {
                    bool found = false;
                    for (int j = 0; j < notBanned.Length; j++)
                    {
                        if (notBanned[j].Name.Equals(allScores[i].Name))
                        {
                            found = true;
                            scores.Add(allScores[i]);
                            break;
                        }
                        if (found) break;
                    }
                    if (allScores[i]._id == entry._id) break;
                }
                EntryResult result = new EntryResult();
                result.Name = entry.Name;
                result.Banned = false;
                result.Score = entry.Score;
                result.Ranking = 0;     //these are initialized just in case the score didn't
                result.BestRanking = 0; // the score didn't make it to the whole list (list is
                result.BestScore = entry.Score; //at max size and the score was lower than the last one)
                                                //and players hasn't any other scores in the list.

                bool bestFound = false;
                bool thisFound = false;
                for (int i = 0; i < scores.Count; i++)
                {
                    if (scores[i].Name.Equals(entry.Name))
                    {
                        if (!bestFound)
                        {
                            bestFound = true;
                            result.BestScore = scores[i].Score;
                            result.BestRanking = i + 1;
                        }
                        if (scores[i]._id == entry._id)
                        {
                            thisFound = true;
                            result.Ranking = i + 1;
                        }
                    }
                    if (thisFound) break;
                }

                return result;

            }
        }
    }
}