using UnityEngine;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int coins;

    public LeaderboardEntry(string name, int coins)
    {
        this.playerName = name;
        this.coins = coins;
    }
}
