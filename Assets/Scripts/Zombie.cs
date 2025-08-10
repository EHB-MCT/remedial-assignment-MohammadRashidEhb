using UnityEngine;
using UnityEngine.UI;

public class Zombie : MonoBehaviour
{
    public int maxHealth = 2;
    private int currentHealth;
    public Image healthBarFill; // In this field, I will add the visual healthbar later 

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        Debug.Log("Zombie spawned with health: " + currentHealth);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        UpdateHealthBar();
        Debug.Log("Zombie took damage: " + dmg + ", current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Zombie died. Requesting GameManager to spawn a new zombie after delay.");

            GameManager.Instance.AddMoney(5);  // Add 5$ for this kill
            GameManager.Instance.SpawnZombieAfterDelay(0.5f);
            Destroy(gameObject);
        }
    }


    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}
