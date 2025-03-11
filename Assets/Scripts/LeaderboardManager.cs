using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class LeaderboardManager : MonoBehaviour
{
    public Transform positionTransform; // Parent transform for leaderboard UI
    public GameObject positionHolderPrefab; // Prefab for displaying leaderboard entries

    private string leaderboardName = "HighScoreLeaderboard"; // Leaderboard name in PlayFab
    public static LeaderboardManager Singleton { get; private set; }
    [HideInInspector] public int previousHighScore;

    private int playerRank = -1; // Player's exact rank

    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        // GetLeaderboard(); // Load main leaderboard
        if (PlayFabClientAPI.IsClientLoggedIn())
        {
            Debug.Log("Player is already logged in.");
            GetPlayerScore();
        }
        else
        {
            return;
        }
    }

    public void SubmitScore(int score)
    {
        if (!PlayFabClientAPI.IsClientLoggedIn()) { Debug.Log("Client not logged in..."); return; }

        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnScoreSubmitted, OnScoreSubmitFailed);
    }

    void OnScoreSubmitted(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Score submitted successfully!");
        // GetLeaderboard(); // Refresh leaderboard after submitting score
    }

    void OnScoreSubmitFailed(PlayFabError error)
    {
        Debug.LogError("Failed to submit score: " + error.GenerateErrorReport());
    }

    public void GetLeaderboard()
    {
        try
        {
            var request = new GetLeaderboardRequest
            {
                StatisticName = leaderboardName,
                StartPosition = 0, // Get from top rank
                MaxResultsCount = 12 // Get top 12 players
            };
            PlayFabClientAPI.GetLeaderboard(request, OnLeaderboardReceived, OnLeaderboardFailed);
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void OnLeaderboardReceived(GetLeaderboardResult result)
    {
        ClearLeaderboard();

        foreach (var entry in result.Leaderboard)
        {
            GameObject newEntry = Instantiate(positionHolderPrefab, positionTransform);
            LeaderboardEntryUI entryUI = newEntry.GetComponent<LeaderboardEntryUI>();

            string playerName = string.IsNullOrEmpty(entry.DisplayName) ? "Guest" : entry.DisplayName;

            if (entryUI != null)
            {
                entryUI.SetLeaderboardEntry(entry.Position + 1, playerName, entry.StatValue, false);
            }

            if (entry.PlayFabId == PlayFabSettings.staticPlayer.PlayFabId)
            {
                playerRank = entry.Position; // Store player's rank
            }
        }

        StartCoroutine(LoadPlayerLeaderboardAfterDelay(2.5f));
    }


    void OnLeaderboardFailed(PlayFabError error)
    {
        Debug.LogError("Failed to retrieve leaderboard: " + error.GenerateErrorReport());
    }

    IEnumerator LoadPlayerLeaderboardAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GetPlayerLeaderboard();
    }

   void GetPlayerLeaderboard()
{
    if (playerRank != -1)
    {
        // Player is already in the main leaderboard, so just fetch nearby players
        int startPos = Mathf.Max(0, playerRank - 4); // Get ±4 players around player's rank

        var request = new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            StartPosition = startPos,
            MaxResultsCount = 8
        };

        PlayFabClientAPI.GetLeaderboard(request, OnPlayerLeaderboardReceived, OnLeaderboardFailed);
    }
    else
    {
        // Player is NOT in the main leaderboard; fetch their exact rank
        GetExactPlayerRank();
    }
}

void GetExactPlayerRank()
{
    var request = new GetLeaderboardAroundPlayerRequest
    {
        StatisticName = leaderboardName,
        MaxResultsCount = 8 // Get 8 closest players
    };

    PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnExactPlayerRankReceived, OnLeaderboardFailed);
}

void OnExactPlayerRankReceived(GetLeaderboardAroundPlayerResult result)
{
    ClearLeaderboard();

    foreach (var entry in result.Leaderboard)
    {
        GameObject newEntry = Instantiate(positionHolderPrefab, positionTransform);
        LeaderboardEntryUI entryUI = newEntry.GetComponent<LeaderboardEntryUI>();

        if (entryUI != null)
        {
            bool isPlayer = (entry.PlayFabId == PlayFabSettings.staticPlayer.PlayFabId);
            entryUI.SetLeaderboardEntry(entry.Position + 1, entry.DisplayName, entry.StatValue, isPlayer);
        }

        if (entry.PlayFabId == PlayFabSettings.staticPlayer.PlayFabId)
        {
            playerRank = entry.Position;
            Debug.Log($"Player's Exact Rank: {playerRank + 1}");
        }
    }
}


    void OnPlayerLeaderboardReceived(GetLeaderboardResult result)
    {
        ClearLeaderboard();

        foreach (var entry in result.Leaderboard)
        {
            GameObject newEntry = Instantiate(positionHolderPrefab, positionTransform);
            LeaderboardEntryUI entryUI = newEntry.GetComponent<LeaderboardEntryUI>();

            if (entryUI != null)
            {
                bool isPlayer = (entry.PlayFabId == PlayFabSettings.staticPlayer.PlayFabId);
                entryUI.SetLeaderboardEntry(entry.Position + 1, entry.DisplayName, entry.StatValue, isPlayer);
            }
        }
    }

    void ClearLeaderboard()
    {
        foreach (Transform child in positionTransform)
        {
            Destroy(child.gameObject);
        }
    }

    //-- Getting Player Exact Score --- //

    public void GetPlayerScore()
    {
        var request = new GetPlayerStatisticsRequest();

        PlayFabClientAPI.GetPlayerStatistics(request, OnPlayerScoreReceived, OnPlayerScoreFailed);
    }

    void OnPlayerScoreReceived(GetPlayerStatisticsResult result)
    {
        foreach (var stat in result.Statistics)
        {
            if (stat.StatisticName == leaderboardName) // "HighScores"
            {
                Debug.Log("Player Score: " + stat.Value);
                previousHighScore = stat.Value;
                return;
            }
        }

        Debug.Log("Player has no recorded score yet.");
    }

    void OnPlayerScoreFailed(PlayFabError error)
    {
        Debug.LogError("Failed to retrieve player score: " + error.GenerateErrorReport());
    }
}