using UnityEngine;
using TMPro;
using Firebase.Database;
using System.Collections;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;

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

    // SHA256 Hashing function
    public static string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    // Register new user
    public void OnRegisterButtonClicked()
    {
        string playerName = playerNameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(password))
        {
            feedbackText.text = "Please enter both fields.";
            return;
        }

        string hashedPassword = HashPassword(password);

        dbRef.Child("users").Child(playerName).Child("password").SetValueAsync(hashedPassword);
        feedbackText.text = "Registered successfully!";

        // Automatically log in the new user
        PlayerPrefs.SetString("CurrentUserName", playerName);
        PlayerPrefs.Save();
        SceneManager.LoadScene("ZombieFarm");
    }

    // Login user
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

    IEnumerator CheckLogin(string playerName, string enteredPassword)
    {
        var task = dbRef.Child("users").Child(playerName).Child("password").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            feedbackText.text = "Error connecting to database.";
            yield break;
        }

        if (task.Result.Exists && task.Result.Value != null)
        {
            string storedHashedPassword = task.Result.Value.ToString();
            string enteredHashedPassword = HashPassword(enteredPassword);

            if (storedHashedPassword == enteredHashedPassword)
            {
                feedbackText.text = "Login successful!";
                PlayerPrefs.SetString("CurrentUserName", playerName);
                PlayerPrefs.Save();
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
