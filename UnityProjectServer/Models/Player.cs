using System;

namespace UnityProjectServer.Models
{
    public class Player
    {
        public Guid _id {get; set;}
        public string Name {get; set;}
        public bool Banned {get; set;}

    }
}