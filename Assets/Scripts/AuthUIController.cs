using UnityEngine;
using TMPro;
using Firebase.Database;
using System.Collections;
using UnityEngine.SceneManagement;

// Simple username/password-based authentication UI controller
// using Firebase Realtime Database
public class AuthUIController : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public TMP_InputField passwordInput;
    public TMP_Text feedbackText;
    DatabaseReference dbRef;

    void Awake()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // This wil be called when the Register =button is pressed 
    public void OnRegisterButtonClicked()
    {
        string playerName = playerNameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please enter both fields.";
            return;
        }

        // Save player data under playerName as unique key
        dbRef.Child("users").Child(playerName).Child("password").SetValueAsync(password);
        feedbackText.text = "Registered successfully!";

        // Immediately log them in after registration
        PlayerPrefs.SetString("CurrentUserName", playerName);
        PlayerPrefs.Save();
        SceneManager.LoadScene("ZombieFarm");
    }

    // THis will be called when Login button is pressed 
    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please enter both fields.";
            return;
        }

        StartCoroutine(CheckLogin(playerName, password));
    }

    // This coroutine will validate credentials from Firebase 
    IEnumerator CheckLogin(string playerName, string enteredPassword)
    {
        var task = dbRef.Child("users").Child(playerName).Child("password").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Result.Exists && task.Result.Value != null)
        {
            string storedPassword = task.Result.Value.ToString();
            if (storedPassword == enteredPassword)
            {
                feedbackText.text = "Login successful!";
                PlayerPrefs.SetString("CurrentUserName", playerName);
                PlayerPrefs.Save();
                SceneManager.LoadScene("ZombieFarm");


                SceneManager.LoadScene("ZombieFarm");
            }
            else
            {
                feedbackText.text = "Wrong password.";
            }
        }
        else
        {
            feedbackText.text = "User not found.";
        }
    }
}
