using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI scoreText;

    public void SetLeaderboardEntry(int rank, string username, int score, bool isPlayer)
    {
        rankText.text = rank.ToString();
        usernameText.text = string.IsNullOrEmpty(username) ? "Anonymous" : username;
        scoreText.text = score.ToString();

        if (isPlayer)
        {
            rankText.color = Color.red;
            usernameText.color = Color.red;
            scoreText.color = Color.red;
        }
    }
}