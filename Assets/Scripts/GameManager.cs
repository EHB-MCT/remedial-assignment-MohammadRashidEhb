using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // The gamemanager focuses on the core functionality of the game 
    // Handles zombie spawning, money tracking, and central game coordination.
    public static GameManager Instance { get; private set; }

    public GameObject zombiePrefab;
    public Transform zombieSpawnPoint;

    private int money = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SpawnZombie();
    }

    public void SpawnZombie()
    {
        if (zombiePrefab != null && zombieSpawnPoint != null)
        {
            Instantiate(zombiePrefab, zombieSpawnPoint.position, Quaternion.identity);
            Debug.Log("Spawned a new zombie.");
        }
        else
        {
            Debug.LogError("ZombiePrefab or SpawnPoint is not assigned in GameManager.");
        }
    }

    public void SpawnZombieAfterDelay(float delay)
    {
        StartCoroutine(SpawnZombieCoroutine(delay));
    }

    private IEnumerator SpawnZombieCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnZombie();
        Debug.Log("New zombie spawned after delay.");
    }

    // New method to add money when a zombie is killed
    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log("Money added: $" + amount + ". Total money: $" + money);
        // I will add a UI and add the update 
    }

    // This is a test 
    public int GetMoney()
    {
        return money;
    }
}

