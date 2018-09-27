using MongoDB.Driver;
using UnityProjectServer.MongoDB;
using UnityProjectServer.Models;
using System.Threading.Tasks;
using System;

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
            return await cursor.FirstAsync();
        }

        public async Task<Player> GetPlayerWithID (Guid id)
        {
            var filter = Builders<Player>.Filter.Eq(p => p._id, id);
            var cursor = await _players.FindAsync(filter);
            return await cursor.FirstAsync();
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
            var player = await cursor.FirstAsync();
            return player.Banned;
        }

        public async Task<Player> SetBannedStatus(Player player)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Name, player.Name);
            await _players.ReplaceOneAsync(filter, player);
            var cursor = await _players.FindAsync(filter);
            return await cursor.FirstAsync();
        }

        public async Task<ScoreEntry[]> TopTen()
        {
            var sort = Builders<ScoreEntry>.Sort.Descending("Score");
            var cursor = _scores.Find(_=> true).Sort(sort).Limit(10);
            var scores = await cursor.ToListAsync();
            return scores.ToArray();
        }

        public async Task<Player> SetBannedStatusWithName(string name, bool banned)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Name, name);
            var set = Builders<Player>.Update.Set(p => p.Banned, banned);
            await _players.FindOneAndUpdateAsync(filter, set);
            var cursor = await _players.FindAsync(filter);
            return await cursor.FirstAsync();
        }

        public async Task<Player> ChangeBannedStatusWithName(string name)
        {
            var filter = Builders<Player>.Filter.Eq(p => p.Name, name);
            var cursor = await _players.FindAsync(filter);
            var player = await cursor.FirstAsync();
            if (player.Banned)
            {
                player.Banned = false;
            }
            else
            {
                player.Banned = true;
            }
            return await _players.FindOneAndReplaceAsync(filter, player);
        }

        public async Task<Player> SetBannedStatusWithID(Guid id, bool banned)
        {
            var filter = Builders<Player>.Filter.Eq(p => p._id, id);
            var set = Builders<Player>.Update.Set(p => p.Banned, banned);
            await _players.FindOneAndUpdateAsync(filter, set);
            var cursor = await _players.FindAsync(filter);
            return await cursor.FirstAsync();
        }

        public async Task<Player> ChangeBannedStatusWithID(Guid id)
        {
            var filter = Builders<Player>.Filter.Eq(p => p._id, id);
            var cursor = await _players.FindAsync(filter);
            var player = await cursor.FirstAsync();
            if (player.Banned)
            {
                player.Banned = false;
            }
            else
            {
                player.Banned = true;
            }
            return await _players.FindOneAndReplaceAsync(filter, player);
        }
    }
}