using UnityEngine;
using UnityEngine.Events;

public class ZombieBoss : MonoBehaviour
{
    public int maxHealth = 50;
    private int currentHealth;

    // Event to inform GameManager when boss dies
    public UnityEvent onBossDefeated;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Called when boss is clicked by player
    void OnMouseDown()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BossClicked();
        }
        TakeDamage(1); // Increase Damage per wave 
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            if (onBossDefeated != null)
                onBossDefeated.Invoke();

            // Destroy the boss GameObject or play some death animation
            Destroy(gameObject);
        }
    }
}
