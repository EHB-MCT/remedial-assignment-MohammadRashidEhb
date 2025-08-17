using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase.Database;
using System.Linq;
using PimDeWitte.UnityMainThreadDispatcher; // For ordering and reversing

public class LeaderboardManager : MonoBehaviour
{
    [Header("Firebase")]
    private DatabaseReference dbRef;

    [Header("UI")]
    public Transform leaderboardContent;           // Assign the Content transform of your ScrollView in Inspector
    public LeaderboardEntry entryPrefab;           // Assign your row prefab in Inspector

    [Header("Settings")]
    public int entriesToShow = 10;                // How many top scores to display

    private void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        FetchLeaderboardData();

        for (int i = 0; i < 5; i++)
    {
        var entry = Instantiate(entryPrefab, leaderboardContent);
        entry.SetUp(i + 1, "TestUser" + i, 1000 - i * 100);
    }
    }

    public void FetchLeaderboardData()
    {
        // Leaderboard example using highest money
    dbRef.Child("users")
        .OrderByChild("money")
        .LimitToLast(entriesToShow)
        .GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to fetch leaderboard: " + task.Exception);
                return;
            }

            if (task.Result != null && task.Result.ChildrenCount > 0)
            {
                // Gather all leaderboard entries
                var tempList = new List<UserLeaderboardEntry>();

                foreach (var child in task.Result.Children)
                {
                    Debug.Log("User found: " + child.Key);
                    string username = child.Key;
                    int money = 0;
                    if (child.Child("money").Value != null)
                        int.TryParse(child.Child("money").Value.ToString(), out money);

                    tempList.Add(new UserLeaderboardEntry
                    {
                        username = username,
                        money = money
                    });
                }

                // Order descending
                var sortedList = tempList.OrderByDescending(user => user.money).ToList();

                // Pass to UI thread
                UnityMainThreadDispatcher.Instance().Enqueue(() => PopulateLeaderboard(sortedList));
            }
        });
    }

    // Clear the content and fill with new entries
    void PopulateLeaderboard(List<UserLeaderboardEntry> entries)
    {
        // Remove previous entries
        foreach (Transform child in leaderboardContent)
            Destroy(child.gameObject);

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = Instantiate(entryPrefab, leaderboardContent);
            entry.SetUp((i + 1), entries[i].username, entries[i].money);
            Debug.Log($"Instantiating UI for {entries[i].username} with {entries[i].money}");
            
        }
    }

    // Structure to hold fetched leaderboard info
    public class UserLeaderboardEntry
    {
        public string username;
        public int money;
    }
}
