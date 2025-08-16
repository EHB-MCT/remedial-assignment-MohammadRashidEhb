using UnityEngine;
using UnityEngine.UI;

// Basic zombie enemy logic: health management, damage response, and dying.
public class Zombie : MonoBehaviour
{
    public int maxHealth = 2;
    private int currentHealth;
    public Image healthBarFill;

    // NEW: track which lane this zombie belongs to
    public int laneNumber;
    

    void Start()
    {
        if (laneNumber <= 0)
        {
            Debug.LogError($"{name} spawned without a valid laneNumber!");
        }
        currentHealth = maxHealth;
        UpdateHealthBar();
        Debug.Log($"Zombie spawned with health: {currentHealth} in lane {laneNumber}");
    }

    // This will be called when the bullet hits 
    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        UpdateHealthBar();
        Debug.Log($"Zombie took damage: {dmg}, current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Zombie died. Respawning in same lane.");
            GameManager.Instance.AddMoney(5);
            GameManager.Instance.IncrementKill();
            GameManager.Instance.SpawnZombieInLaneAfterDelay(laneNumber, 0.5f);
            Destroy(gameObject);
        }
    }

    // I will add the UI visuals later
    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}
