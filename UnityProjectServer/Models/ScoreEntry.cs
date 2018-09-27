using System;

namespace UnityProjectServer.Models
{
    public class ScoreEntry
    {
        public Guid _id {get; set;}
        public string Name {get; set;}
        public int Score {get; set;}
        public DateTime Date {get; set;}

    }

    public class NewEntry
    {
        public string Name {get; set;}
        public int Score {get; set;}
    }

    public class EntryResult
    {
        public string Name {get; set;}
        public bool Banned {get; set;}

        public int Score {get; set;}
        public int Ranking {get; set;}
        public int BestScore {get; set;}
        public int BestRanking {get; set;}
    }

}