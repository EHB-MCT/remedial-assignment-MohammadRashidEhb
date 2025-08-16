using UnityEngine;
using TMPro;

// In this class, I made a simple Killcounter UI (made it public so gamemanager can access it)
public class KillsUIController : MonoBehaviour
{
    public TMP_Text killsText;

    // This will be called every time the kills are updated
    public void UpdateKills(int kills, int killsToWave)
    {
        if (killsText != null)
            killsText.text = $"Kills: {kills}/{killsToWave}";
    }
}
