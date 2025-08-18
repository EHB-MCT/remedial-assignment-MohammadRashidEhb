using UnityEngine;
using TMPro;

public class BossFightUIController : MonoBehaviour
{
    public GameObject bossUIPanel; // Parent Panel
    public TMP_Text bossClicksText;
    public TMP_Text bossPrepText;
    public TMP_Text bossTimerText;
    public TMP_Text bossBonusText;

    // Show only the prep text
    public void ShowPrepUI()
    {
        if (bossUIPanel != null) bossUIPanel.SetActive(true);
        if (bossPrepText != null) bossPrepText.gameObject.SetActive(true);
        if (bossClicksText != null) bossClicksText.gameObject.SetActive(false);
        if (bossTimerText != null) bossTimerText.gameObject.SetActive(false);
        if (bossBonusText != null) bossBonusText.gameObject.SetActive(false);
    }

    // Show only the active boss fight texts (no prep)
    public void ShowBossFightUI()
    {
        if (bossUIPanel != null) bossUIPanel.SetActive(true);
        if (bossPrepText != null) bossPrepText.gameObject.SetActive(false);
        if (bossClicksText != null) bossClicksText.gameObject.SetActive(true);
        if (bossTimerText != null) bossTimerText.gameObject.SetActive(true);
        if (bossBonusText != null) bossBonusText.gameObject.SetActive(true);
    }

    // Hide all boss UI
    public void HideAllBossUI()
    {
        if (bossUIPanel != null) bossUIPanel.SetActive(false);
    }

    // Update boss click count visible text
    public void UpdateBossClicks(int clickCount)
    {
        if (bossClicksText != null) bossClicksText.text = $"Boss Clicks: {clickCount}";
    }

    // Update bonus/multiplier visible text
    public void UpdateBossBonus(int baseMoney, int clickCount, float perClickBonus)
    {
        if (bossBonusText != null)
        {
            float mult = 1f + clickCount * perClickBonus;
            int bonus = Mathf.RoundToInt(baseMoney * (mult - 1f));
            bossBonusText.text = $"Multiplier: x{mult:F1}\nBonus: €{bonus}";
        }
    }

    public void UpdateBossBonusOnly(int bonusAmount)
    {
        if (bossBonusText != null)
        {
            bossBonusText.text = $"Bonus: €{bonusAmount}";
        }
    }

    // Update timer text for boss fight phase
    public void UpdateTimer(float timeRemaining)
    {
        if (bossTimerText != null) bossTimerText.text = $"Time Left: {Mathf.CeilToInt(timeRemaining)}s";
    }
}
