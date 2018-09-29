using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityProjectServer.Models;

namespace UnityProjectServer.Repositories
{
    public interface IRepository
    {
        Task<ScoreEntry[]> GetScoresDescending(int days);
        Task<EntryResult> AddNewEntry(ScoreEntry entry);

        Task<Player[]> GetAllPlayers();

        Task<Player[]> GetPlayersWithBannedStatus(bool status);
        Task<Player> SetBannedStatus(Player player);
        Task<Player> GetPlayer(string name);
        Task <bool> AddNewPlayerIfNecessary(string name);
        Task<bool> IsPlayerBanned(string name);
        Task<Player> GetPlayerWithID (Guid id);
        Task<Player> SetBannedStatusWithName(string name, bool banned);
        Task<Player> ChangeBannedStatusWithName(string name);
        Task<Player> SetBannedStatusWithID(Guid id, bool banned);
        Task<Player> ChangeBannedStatusWithID(Guid id);
        Task<ScoreEntry[]> GetPlayersScores(string name, int timeFrame);
        Task <bool> DeletePlayersScores(string name);
        Task<bool> DeletePlayer(Guid id);

    }

}