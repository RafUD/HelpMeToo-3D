using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    public int maxEntries = 10;

    private const string LEADERBOARD_KEY = "LEADERBOARD";

    [System.Serializable]
    private class LeaderboardWrapper
    {
        public List<LeaderboardEntry> entries = new();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveScore(int coins)
    {
        string playerName = PlayerPrefs.GetString("PlayerID", "Player");

        LeaderboardWrapper leaderboard = LoadWrapper();

        // Find existing entry
        var existing = leaderboard.entries
            .FirstOrDefault(e => e.playerName == playerName);

        if (existing != null)
        {
            if (coins > existing.coins)
                existing.coins = coins;
        }
        else
        {
            leaderboard.entries.Add(new LeaderboardEntry(playerName, coins));
        }

        leaderboard.entries = leaderboard.entries
            .OrderByDescending(e => e.coins)
            .Take(maxEntries)
            .ToList();

        PlayerPrefs.SetString(
            LEADERBOARD_KEY,
            JsonUtility.ToJson(leaderboard)
        );
        PlayerPrefs.Save();
    }

    public List<LeaderboardEntry> LoadLeaderboard()
    {
        return LoadWrapper().entries;
    }

    private LeaderboardWrapper LoadWrapper()
    {
        if (!PlayerPrefs.HasKey(LEADERBOARD_KEY))
            return new LeaderboardWrapper();

        string json = PlayerPrefs.GetString(LEADERBOARD_KEY);
        return JsonUtility.FromJson<LeaderboardWrapper>(json);
    }
}
