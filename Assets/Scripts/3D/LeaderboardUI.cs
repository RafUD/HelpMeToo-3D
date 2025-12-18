using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;

public class LeaderboardUI : MonoBehaviour
{
    public TMP_Text leaderboardText;

    void OnEnable()
    {
        // Delay refresh to ensure LeaderboardManager is initialized
        StartCoroutine(DelayedRefresh());
    }

    private IEnumerator DelayedRefresh()
    {
        // Wait one frame to ensure all Awake() calls have completed
        yield return null;
        Refresh();
    }

    public void Refresh()
    {
        if (LeaderboardManager.Instance == null)
        {
            Debug.LogError("[LeaderboardUI] LeaderboardManager not found in scene!");
            return;
        }

        var entries = LeaderboardManager.Instance.LoadLeaderboard();
        StringBuilder sb = new();
        sb.AppendLine("<b>LEADERBOARD</b>\n");

        for (int i = 0; i < entries.Count; i++)
        {
            sb.AppendLine(
                $"{i + 1}. {entries[i].playerName} - {entries[i].coins}"
            );
        }

        if (entries.Count == 0)
        {
            sb.AppendLine("<i>No scores yet!</i>");
        }

        leaderboardText.text = sb.ToString();
    }
}