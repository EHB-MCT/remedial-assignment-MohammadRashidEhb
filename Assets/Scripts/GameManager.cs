using System.Collections;
using UnityEngine;
using Firebase.Database;

// The gamemanager focuses on the core functionality of the game 
// Handles zombie spawning, money tracking, and central game coordination.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject zombiePrefab;
    public MoneyUIController moneyUIController;
    private int money = 0;

    // Track survivor ownership per lane
    private bool[] survivorsInLane = new bool[3];

    // Survivor prefab to spawn when purchased
    public GameObject survivorPrefab;

    // Positions in the scene where each survivor/zombie should spawn
    public Transform[] survivorLanePositions;
    public Transform[] zombieLanePositions;

    // Survivor cost (for now fixed for all types)
    public int survivorCost = 20;

    private DatabaseReference dbRef;
    public string currentUserName; // The logged-in username

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // init firebase reference
            dbRef = FirebaseDatabase.DefaultInstance.RootReference;

            // retrieve username from PlayerPrefs
            currentUserName = PlayerPrefs.GetString("CurrentUserName", "");
            if (string.IsNullOrEmpty(currentUserName))
            {
                Debug.LogError("No username found! Make sure AuthUIController saves it before loading game scene.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load user's saved money first, then start spawning
        StartCoroutine(LoadMoneyAndInitializeGame());
    }

    private IEnumerator LoadMoneyAndInitializeGame()
    {
        if (string.IsNullOrEmpty(currentUserName))
        {
            money = 0;
        }
        else
        {
            var moneyTask = dbRef.Child("users").Child(currentUserName).Child("money").GetValueAsync();
            yield return new WaitUntil(() => moneyTask.IsCompleted);

            if (moneyTask.Exception != null)
            {
                Debug.LogError("Failed to load user's money: " + moneyTask.Exception);
                money = 0;
            }
            else
            {
                var snap = moneyTask.Result;
                if (snap.Exists && int.TryParse(snap.Value.ToString(), out int value))
                {
                    money = value;
                }
                else
                {
                    money = 0; // start fresh if no data
                }
            }
        }

        // update UI with loaded money
        if (moneyUIController != null)
            moneyUIController.UpdateMoney(money);

        // Spawn zombies in all lanes at start
        SpawnAllZombiesAtStart();

        // Lane 1 starts with a survivor for free
        survivorsInLane[0] = false; // ensures BuySurvivor works
        BuySurvivor(1, 0); // 0 cost for first survivor
    }


    // Spawns a zombie in a specific lane
    public void SpawnZombieInLane(int laneNumber)
    {
        int laneIndex = laneNumber - 1;
        if (laneIndex < 0 || laneIndex >= zombieLanePositions.Length)
        {
            Debug.LogError("Invalid lane number for spawning zombie: " + laneNumber);
            return;
        }

        if (zombiePrefab != null && zombieLanePositions[laneIndex] != null)
        {
            GameObject newZombie = Instantiate(zombiePrefab,
                zombieLanePositions[laneIndex].position,
                Quaternion.identity);

            // Assign lane number to the zombie script
            Zombie zombieScript = newZombie.GetComponent<Zombie>();
            if (zombieScript != null)
            {
                zombieScript.laneNumber = laneNumber;
            }

            Debug.Log($"Spawned a new zombie in lane {laneNumber}.");
        }
        else
        {
            Debug.LogError("ZombiePrefab or lane spawn point not assigned for lane " + laneNumber);
        }
    }

    // Spawns a zombie after a delay (used when zombie dies)
    public void SpawnZombieInLaneAfterDelay(int laneNumber, float delay)
    {
        StartCoroutine(SpawnZombieLaneCoroutine(laneNumber, delay));
    }

    private IEnumerator SpawnZombieLaneCoroutine(int laneNumber, float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnZombieInLane(laneNumber);
        Debug.Log($"New zombie spawned in lane {laneNumber} after delay.");
    }

    // Spawns one zombie in each lane
    public void SpawnAllZombiesAtStart()
    {
        for (int lane = 1; lane <= 3; lane++)
        {
            SpawnZombieInLane(lane);
        }
    }

    // New method to add money when a zombie is killed
    public void AddMoney(int amount)
    {
        money += amount;
        Debug.Log("Money added: €" + amount + ". Total money: €" + money);
        if (moneyUIController != null)
            moneyUIController.UpdateMoney(money);
        SaveMoneyToFirebase(); // save to database after change
    }

    // save current money to firebase under this user
    private void SaveMoneyToFirebase()
    {
        if (string.IsNullOrEmpty(currentUserName))
        {
            Debug.LogError("Current username empty, can't save money.");
            return;
        }

        dbRef.Child("users").Child(currentUserName).Child("money").SetValueAsync(money)
            .ContinueWith(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError("Error saving money to Firebase: " + task.Exception);
                }
                else
                {
                    Debug.Log("Saved money to Firebase: €" + money);
                }
            });
    }

    public int GetMoney()
    {
        return money;
    }

    // I created a Public property to allow UI scripts to check money
    public int Money => money;

    public bool BuySurvivor(int laneNumber, int cost)
    {
        int laneIndex = laneNumber - 1;

        // Check for valid lane index
        if (laneIndex < 0 || laneIndex >= survivorsInLane.Length)
        {
            Debug.LogError("Invalid lane number: " + laneNumber);
            return false;
        }

        // Check if survivor already exists in lane
        if (survivorsInLane[laneIndex])
        {
            Debug.Log("Lane " + laneNumber + " already has a survivor.");
            return false;
        }

        // Check if player has enough money
        if (money >= cost)
        {
            money -= cost;
            survivorsInLane[laneIndex] = true;

            // Spawn survivor in chosen lane position
            if (survivorPrefab != null && survivorLanePositions[laneIndex] != null)
            {
                Instantiate(survivorPrefab, survivorLanePositions[laneIndex].position, Quaternion.identity);
                Debug.Log($"Survivor spawned in lane {laneNumber}.");
            }
            else
            {
                Debug.LogError("Survivor prefab or lane position not assigned for lane " + laneNumber);
            }

            // Update money in UI
            if (moneyUIController != null)
                moneyUIController.UpdateMoney(money);
            SaveMoneyToFirebase(); // save money change
            return true;
        }
        else
        {
            Debug.Log("Not enough money to buy survivor.");
            return false;
        }
    }
}
