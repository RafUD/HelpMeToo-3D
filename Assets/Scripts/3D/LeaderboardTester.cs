using UnityEngine;

public class LeaderboardTester : MonoBehaviour
{
    void Start()
    {
        // Set a player name
        PlayerPrefs.SetString("PlayerID", "TestPlayer");

        // Add some test scores
        LeaderboardManager.Instance.SaveScore(100);
        LeaderboardManager.Instance.SaveScore(250);
        LeaderboardManager.Instance.SaveScore(150);
    }

    void Update()
    {
        // Press T to add a random score
        if (Input.GetKeyDown(KeyCode.T))
        {
            int randomScore = Random.Range(50, 500);
            LeaderboardManager.Instance.SaveScore(randomScore);
            Debug.Log($"Added score: {randomScore}");

            // Refresh UI if it exists
            var ui = FindAnyObjectByType<LeaderboardUI>();
            if (ui != null) ui.Refresh();
        }
    }
}