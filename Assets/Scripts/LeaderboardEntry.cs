using UnityEngine;
using TMPro;

public class LeaderboardEntry : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text usernameText;
    public TMP_Text scoreText;

    // Call this on instantiation
    public void SetUp(int rank, string username, int score)
    {
        if (rankText != null) rankText.text = "#" + rank;
        if (usernameText != null) usernameText.text = username;
        if (scoreText != null) scoreText.text = score.ToString();
    }
}
