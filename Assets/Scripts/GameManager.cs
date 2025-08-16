using System.Collections;
using UnityEngine;
using Firebase.Database;
using TMPro;

// The gamemanager focuses on the core functionality of the game 
// Handles zombie spawning, money tracking, and central game coordination.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameObject zombiePrefab;
    public MoneyUIController moneyUIController;
    private int money = 0;

    // For wave/kills
    public KillsUIController killsUIController; 
    public TMP_Text waveCompleteText; 
    private int currentWave = 1; 
    public int killsToWave = 20;
    private int killsThisWave = 0;
    
    // Track survivor ownership per lane
    private int[] survivorTypeInLane = new int[3]; 

    // This is to Keep track of spawned survivor instances so we can destroy on sell
    private GameObject[] survivorInstances = new GameObject[3];

    // Survivor prefabs for each type (assign in Inspector)
    public GameObject survivorTypeA_Prefab;
    public GameObject survivorTypeB_Prefab;
    public GameObject survivorTypeC_Prefab;

    // Positions in the scene where each survivor/zombie should spawn
    public Transform[] survivorLanePositions;
    public Transform[] zombieLanePositions;

    // Cost configuration
    public int costTypeA = 20;
    public int costTypeB = 40;
    public int costTypeC = 60;
    public int sellRefundPercent = 50; // Percent of buy price refunded

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

        // Initialize kills UI
        if (killsUIController != null)
            killsUIController.UpdateKills(killsThisWave, killsToWave);
    }

    private IEnumerator LoadMoneyAndInitializeGame()
    {
       if (!string.IsNullOrEmpty(currentUserName))
        {
            var moneyTask = dbRef.Child("users").Child(currentUserName).Child("money").GetValueAsync();
            yield return new WaitUntil(() => moneyTask.IsCompleted);

            if (moneyTask.Exception == null && moneyTask.Result.Exists &&
                int.TryParse(moneyTask.Result.Value.ToString(), out int loadedMoney))
            {
                money = loadedMoney;
            }
            else
            {
                money = 0;
            }
        }

        // update UI with loaded money
        if (moneyUIController != null)
            moneyUIController.UpdateMoney(money);

         // Load survivors from database
        if (!string.IsNullOrEmpty(currentUserName))
        {
            var survivorTask = dbRef.Child("users").Child(currentUserName).Child("survivors").GetValueAsync();
            yield return new WaitUntil(() => survivorTask.IsCompleted);

            if (survivorTask.Exception == null && survivorTask.Result.Exists)
            {
                foreach (var laneData in survivorTask.Result.Children)
                {
                    int laneIndex = int.Parse(laneData.Key.Replace("lane", ""));
                    int type = int.Parse(laneData.Value.ToString());
                    if (type > 0)
                        SpawnSurvivorInLane(laneIndex + 1, type);
                }
            }
        }

        // Spawn zombies in all lanes at start
        SpawnAllZombiesAtStart();

        // This will ensure lane 1 has at least a TypeA survivor if empty
        if (survivorTypeInLane[0] == 0)
            BuySurvivor(1, 1, 0); // free TypeA survivor
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

        dbRef.Child("users").Child(currentUserName).Child("money").SetValueAsync(money);
    }

    // Wave system, => Increment kills, OnwaveComplete will be called when wave is finished
    public void IncrementKill()
    {
        killsThisWave++;
        if (killsUIController != null)
            killsUIController.UpdateKills(killsThisWave, killsToWave);

        if (killsThisWave >= killsToWave)
        {
            OnWaveComplete();
        }
    }

    // Display wavecomplete message + increment next wave with 10 more zombies
    private void OnWaveComplete()
    {
        if (waveCompleteText != null)
        {
            waveCompleteText.gameObject.SetActive(true);
            waveCompleteText.text = $"Wave {currentWave} Complete!"; // use currentWave variable
            StartCoroutine(HideWaveCompleteMessage());
        }

        killsThisWave = 0; // Reset kill count for next wave
        currentWave++; // increment wave count (declare and initialize at start)
        killsToWave += 10;

        if (killsUIController != null)
            killsUIController.UpdateKills(killsThisWave, killsToWave);

    }

    // Hide Wavecomplete message after delay
    private IEnumerator HideWaveCompleteMessage()
    {
        yield return new WaitForSeconds(2.0f);
        if (waveCompleteText != null)
            waveCompleteText.gameObject.SetActive(false);
    }

    public int Money => money;

    // Buy & Sell survivor
    public bool BuySurvivor(int laneNumber, int survivorType, int cost)
    {
        int laneIndex = laneNumber - 1;
        if (laneIndex < 0 || laneIndex >= survivorTypeInLane.Length) return false;

        if (survivorTypeInLane[laneIndex] != 0)
        {
            Debug.Log($"Lane {laneNumber} already has a survivor.");
            return false;
        }

        if (money < cost)
        {
            Debug.Log("Not enough money to buy survivor.");
            return false;
        }

        money -= cost;
        survivorTypeInLane[laneIndex] = survivorType;

        SpawnSurvivorInLane(laneNumber, survivorType);

        if (moneyUIController != null)
            moneyUIController.UpdateMoney(money);

        SaveMoneyToFirebase();
        SaveSurvivorsToFirebase();
        return true;
    }

    private void SpawnSurvivorInLane(int laneNumber, int survivorType)
    {
        int laneIndex = laneNumber - 1;
        GameObject prefabToSpawn = GetSurvivorPrefab(survivorType);

        if (prefabToSpawn != null && survivorLanePositions[laneIndex] != null)
        {
            GameObject spawned = Instantiate(prefabToSpawn, survivorLanePositions[laneIndex].position, Quaternion.identity);
            survivorInstances[laneIndex] = spawned;
            survivorTypeInLane[laneIndex] = survivorType;
        }
        else
        {
            Debug.LogError("Survivor prefab or lane position missing.");
        }
    }

    public bool SellSurvivor(int laneNumber)
    {
        int laneIndex = laneNumber - 1;
        if (laneIndex < 0 || laneIndex >= survivorTypeInLane.Length) return false;

        int ownedType = survivorTypeInLane[laneIndex];
        if (ownedType == 0)
        {
            Debug.Log($"No survivor to sell in lane {laneNumber}.");
            return false;
        }

        // Destroy instance
        if (survivorInstances[laneIndex] != null)
        {
            Destroy(survivorInstances[laneIndex]);
            survivorInstances[laneIndex] = null;
        }

        // Refund
        int originalCost = GetSurvivorCost(ownedType);
        int refund = (originalCost * sellRefundPercent) / 100;
        money += refund;

        survivorTypeInLane[laneIndex] = 0;

        if (moneyUIController != null)
            moneyUIController.UpdateMoney(money);

        SaveMoneyToFirebase();
        SaveSurvivorsToFirebase();

        Debug.Log($"Sold survivor type {ownedType} in lane {laneNumber} for €{refund}.");
        return true;
    }

    // Prefab Helpers
    private GameObject GetSurvivorPrefab(int type)
    {
        return type switch
        {
            1 => survivorTypeA_Prefab,
            2 => survivorTypeB_Prefab,
            3 => survivorTypeC_Prefab,
            _ => null
        };
    }

    private int GetSurvivorCost(int type)
    {
        return type switch
        {
            1 => costTypeA,
            2 => costTypeB,
            3 => costTypeC,
            _ => 0
        };
    }

    // Save/Load Survivors
    private void SaveSurvivorsToFirebase()
    {
        if (string.IsNullOrEmpty(currentUserName)) return;

        for (int i = 0; i < survivorTypeInLane.Length; i++)
        {
            dbRef.Child("users").Child(currentUserName).Child("survivors")
                .Child($"lane{i}").SetValueAsync(survivorTypeInLane[i]);
        }
    }
}
