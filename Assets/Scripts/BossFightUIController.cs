using UnityEngine;
using TMPro;

public class BossFightUIController : MonoBehaviour
{
    public TMP_Text bossClicksText;
    public TMP_Text bossBonusText; // TODO: add a bonustext for the multiplied money

    // These are set by GameManager during boss fight
    public void UpdateBossClicks(int bossClickCount)
    {
        if (bossClicksText != null)
            bossClicksText.text = $"Boss Clicks: {bossClickCount}";
    }

    public void UpdateBossBonus(int baseMoney, int clickCount, float perClickBonus)
    {
        if (bossBonusText != null)
        {
            float mult = 1f + clickCount * perClickBonus;
            int bonus = Mathf.RoundToInt(baseMoney * mult);
            bossBonusText.text = $"Wave Bonus: x{mult:F1} = €{bonus}";
        }
    }

    // This will hide/Show these texts when boss appears/disappears
    public void ShowBossUI(bool show)
    {
        if (bossClicksText != null) bossClicksText.gameObject.SetActive(show);
        if (bossBonusText != null) bossBonusText.gameObject.SetActive(show);
    }
}
