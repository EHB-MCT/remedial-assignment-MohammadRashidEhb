using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpButtonUI : MonoBehaviour
{
    public bool isSpeedBoost; // Assign in Inspector for each button
    public TMP_Text priceText; // Assign in Inspector to show the price

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        UpdatePrice(); // Set right away
    }

    void Update()
    {
        UpdatePrice();
    }

    void UpdatePrice()
    {
        if (gameManager != null && priceText != null)
        {
            int cost = gameManager.GetPowerUpCost(isSpeedBoost);
            priceText.text = "€" + cost;
        }
    }
}
