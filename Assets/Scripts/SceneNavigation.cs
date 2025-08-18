using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour
{
    public void OpenLeaderboardScene()
    {
        SceneManager.LoadScene("LeaderBoard"); // Load Leaderboard scene
    }

     public void ReturnToZombieFarm()
    {
        SceneManager.LoadScene("ZombieFarm"); // Load ZOmbieFarm scene
    }

}
