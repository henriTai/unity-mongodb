using MongoDB.Driver;
using UnityProjectServer.MongoDB;
using UnityProjectServer.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace UnityProjectServer.Repositories
{
    public class MongoDBRepository : IRepository
    {
        private IMongoCollection <ScoreEntry> _scores;
        private IMongoCollection <Player> _players;
        private IMongoDatabase _database;

        private int _maxScoreEntries;

        public MongoDBRepository (MongoDBClient client)
        {
            _database = client.GetDatabase("unity");
            _scores = _database.GetCollection<ScoreEntry>("scores");
            _players = _database.GetCollection<Player>("players");
            _maxScoreEntries = 5000;
        }

        public async Task<EntryResult> AddNewEntry(ScoreEntry entry)
        {
            EntryResult result = new EntryResult();
            result.Name = entry.Name;
            result.Score = entry.Score;
           
            var filter = Builders<ScoreEntry>.Filter.Gte("Score", entry.Score);
            var betterScores = await _scores.Find(filter).ToListAsync();
            bool qualifies;
            if (betterScores.Count == _maxScoreEntries)
            {
                qualifies = false;
            }
            else
            {
                //Inserts new score
                await _scores.InsertOneAsync(entry);
                qualifies = true;
            }
            //Gets the ranking of the score
            filter = Builders<ScoreEntry>.Filter.Eq("_id", entry._id);
            var sort = Builders<ScoreEntry>.Sort.Descending("Score");
            var cursor = _scores.Find(_ => true).Sort(sort);
            var entries = await cursor.ToListAsync();
            if (qualifies)
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i]._id == entry._id)
                    {
                        result.Ranking = i + 1;
                        break;
                    }
                }

            } else
            {
                result.Ranking = 0;
            }

            //Next, the best score & its ranking
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Name.Equals(entry.Name))
                {
                    result.BestScore = entries[i].Score;
                    result.BestRanking = i + 1;
                    break;
                }
            }           

            return result;           
        }

        public async Task <bool> AddNewPlayerIfNecessary(string name)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Name, name);
            var cursor = await _players.FindAsync(filter);
            bool foundMatch = await cursor.AnyAsync();
            if (!foundMatch)
            {
                Player player = new Player();
                player.Name = name;
                player.Banned = false;
                player._id = Guid.NewGuid();
                await _players.InsertOneAsync(player);
            }
            return true;
        }

        public async Task<Player[]> GetAllPlayers()
        {
            var players = await _players.Find(_ => true).ToListAsync();
            return players.ToArray();
        }

        public async Task<Player> GetPlayer(string name)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Name, name);
            var cursor = await _players.FindAsync(filter);
            var player = await cursor.FirstOrDefaultAsync();
            return player;
        }

        public async Task<Player> GetPlayerWithID (Guid id)
        {
            var filter = Builders<Player>.Filter.Eq(p => p._id, id);
            var cursor = await _players.FindAsync(filter);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<Player[]> GetPlayersWithBannedStatus(bool status)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Banned, status);
            var players = await _players.Find(filter).ToListAsync();
            return players.ToArray();
        }

        public async Task<bool> IsPlayerBanned(string name)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Name, name);
            var cursor = await _players.FindAsync(filter);
            var player = await cursor.FirstOrDefaultAsync();
            return player.Banned;
        }

        public async Task<Player> SetBannedStatus(Player player)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Name, player.Name);
            await _players.ReplaceOneAsync(filter, player);
            var cursor = await _players.FindAsync(filter);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<ScoreEntry[]> GetScoresDescending(int days)
        {
            /*
            var sort = Builders<ScoreEntry>.Sort.Descending("Score");
            var cursor = _scores.Find(_=> true).Sort(sort).Limit(10);
            var scores = await cursor.ToListAsync();
            return scores.ToArray();
            */
            FilterDefinition<ScoreEntry> scorefilter;
            if (days <= 0)
            {
                scorefilter = Builders<ScoreEntry>.Filter.Empty;
            }
            else
            {
                DateTime dt = DateTime.UtcNow;
                dt = dt.AddDays(-days);
                DateTime ddt = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0).ToUniversalTime();
                scorefilter = Builders<ScoreEntry>.Filter.Gt(s => s.Date, ddt);
            }
            var filter = Builders<Player>.Filter.Eq("Banned", false);
            var players = await _players.Find(filter).ToListAsync();

            var sort = Builders<ScoreEntry>.Sort.Descending("Score");
            var cursor = _scores.Find(scorefilter).Sort(sort);
            var scores = await cursor.ToListAsync();
            return scores.ToArray();
            
        }

        public async Task<Player> SetBannedStatusWithName(string name, bool banned)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Name, name);
            var set = Builders<Player>.Update.Set(p => p.Banned, banned);
            await _players.FindOneAndUpdateAsync(filter, set);
            var cursor = await _players.FindAsync(filter);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<Player> ChangeBannedStatusWithName(string name)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Name, name);
            var cursor = await _players.FindAsync(filter);
            var player = await cursor.FirstOrDefaultAsync();
            if (player == null) return player;
            if (player.Banned)
            {
                player.Banned = false;
            }
            else
            {
                player.Banned = true;
            }
            await _players.FindOneAndReplaceAsync(filter, player);
            return player;
        }

        public async Task<Player> SetBannedStatusWithID(Guid id, bool banned)
        {
            var filter = Builders<Player>.Filter.Eq(p => p._id, id);
            var set = Builders<Player>.Update.Set(p => p.Banned, banned);
            await _players.FindOneAndUpdateAsync(filter, set);
            var cursor = await _players.FindAsync(filter);
            return await cursor.FirstOrDefaultAsync();
        }

        public async Task<Player> ChangeBannedStatusWithID(Guid id)
        {
            var filter = Builders<Player>.Filter.Eq(p => p._id, id);
            var cursor = await _players.FindAsync(filter);
            var player = await cursor.FirstOrDefaultAsync();
            if (player==null) return player;
            if (player.Banned)
            {
                player.Banned = false;
            }
            else
            {
                player.Banned = true;
            }
            await _players.FindOneAndReplaceAsync(filter, player);
            return player;
        }

        public async Task<ScoreEntry[]> GetPlayersScores(string name, int timeFrame)
        {
            FilterDefinition<ScoreEntry> filter;
            DateTime dt = DateTime.Now;
            dt = dt.AddDays(-timeFrame);

            dt.ToUniversalTime();
            DateTime ddt = new DateTime (dt.Year, dt.Month, dt.Day, 0, 0, 0).ToUniversalTime();
            
            if (timeFrame==0)
            {
                filter = Builders<ScoreEntry>.Filter.Eq("Name", name);
            }
            else
            {
                filter = Builders<ScoreEntry>.Filter.Eq("Name", name) & 
                Builders<ScoreEntry>.Filter.Gt(s => s.Date, ddt);
            }
            var sort = Builders<ScoreEntry>.Sort.Descending("Score");

            var cursor = _scores.Find(filter).Sort(sort);
            var scores = await cursor.ToListAsync();
            return scores.ToArray();
        }

        public async Task<bool> DeletePlayersScores(string name)
        {
            var filter = Builders<ScoreEntry>.Filter.Eq("Name", name);
            await _scores.DeleteManyAsync(filter);
            return true;
        }

        public async Task<bool> DeletePlayer(Guid id)
        {
            var filter = Builders<Player>.Filter.Eq("_id", id);
            await _players.DeleteOneAsync(filter);
            return true;
        }

    }
}