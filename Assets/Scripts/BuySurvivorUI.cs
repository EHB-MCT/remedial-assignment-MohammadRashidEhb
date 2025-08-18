using UnityEngine;

public class BuySurvivorUI : MonoBehaviour
{
    public GameManager gameManager;    // Reference to game manager

    // Costs for each survivor type, can override GameManager defaults here if needed
    public int costTypeA = 20;
    public int costTypeB = 40;
    public int costTypeC = 60;

    // Called from Buy buttons in the UI (
    public void OnBuyTypeA(int laneNumber)
    {
        if (gameManager != null)
        {
            gameManager.BuySurvivor(laneNumber, 1, costTypeA);
        }
        else
        {
            Debug.LogError("GameManager not assigned in BuySurvivorUI.");
        }
    }

    public void OnBuyTypeB(int laneNumber)
    {
        if (gameManager != null)
        {
            gameManager.BuySurvivor(laneNumber, 2, costTypeB);
        }
        else
        {
            Debug.LogError("GameManager not assigned in BuySurvivorUI.");
        }
    }

    public void OnBuyTypeC(int laneNumber)
    {
        if (gameManager != null)
        {
            gameManager.BuySurvivor(laneNumber, 3, costTypeC);
        }
        else
        {
            Debug.LogError("GameManager not assigned in BuySurvivorUI.");
        }
    }

    // This will be called from the Sell button in the UI for each lane
    public void OnSellSurvivor(int laneNumber)
    {
        if (gameManager != null)
        {
            gameManager.SellSurvivor(laneNumber);
        }
        else
        {
            Debug.LogError("GameManager not assigned in BuySurvivorUI.");
        }
    }
}
