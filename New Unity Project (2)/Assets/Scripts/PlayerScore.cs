using System.Collections.Generic;
using System;

[Serializable]
public class Player : object
{

    public Guid _id;
    public string Name;
    public bool Banned;
}

[Serializable]
public class Players : object
{
    public List<Player> Plrs;
}

[Serializable]
public class ScoreEntry : object
{
    public Guid _id;
    public string name;
    public int score;
    public DateTime date;

}

[Serializable]
public class NewEntry : object
{
    public string Name;
    public int Score;
}

[Serializable]
public class EntryResult : object
{
    public string Name;
    public bool Banned;

    public int Score;
    public int Ranking;
    public int BestScore;
    public int BestRanking;
}

[Serializable]
public class Scores : object
{
    public List<ScoreEntry> scores;
}
