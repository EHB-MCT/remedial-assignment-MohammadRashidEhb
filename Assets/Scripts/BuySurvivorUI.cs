using UnityEngine;

public class BuySurvivorUI : MonoBehaviour
{
    public GameManager gameManager;    // Reference to game manager
    public int survivorCost = 20;      // Cost to buy a survivor

    // Call method from button click for each lane number 1-3
    public void OnBuySurvivor(int laneNumber)
    {
        if (gameManager == null)
        {
            Debug.LogError("GameManager reference missing.");
            return;
        }

        bool success = gameManager.BuySurvivor(laneNumber, survivorCost);
        if (success)
        {
            Debug.Log($"Bought survivor for lane {laneNumber}.");
        }
        else
        {
            Debug.Log($"Purchase failed for lane {laneNumber}.");
        }
    }
}
