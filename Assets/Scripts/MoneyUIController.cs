using UnityEngine;
using TMPro;

public class MoneyUIController : MonoBehaviour
{
    public TMP_Text moneyText;

    // Call this method to update money display
    public void UpdateMoney(int amount)
    {
        if (moneyText != null)
            moneyText.text = "€" + amount.ToString();
    }
}
