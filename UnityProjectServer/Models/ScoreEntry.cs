using System;
using System.ComponentModel.DataAnnotations;

namespace UnityProjectServer.Models
{
    public class ScoreEntry
    {
        [Required]
        public Guid _id {get; set;}
        [Required]
        public string Name {get; set;}
        [Required]
        [Range(0, 999999999)]
        public int Score {get; set;}
        [Required]
        public DateTime Date {get; set;}

    }

    public class NewEntry
    {
        [Required (ErrorMessage = "Name is missing.")]
        public string Name {get; set;}
        [Required (ErrorMessage = "Score is missing.")]
        [Range(0, 999999999, ErrorMessage = "Value must be between 0 and 999999999.")]
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