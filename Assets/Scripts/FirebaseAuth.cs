using Firebase.Auth;
using UnityEngine;

// THis is the Central manager for Firebase Authentication.
// Initializes and stores a static FirebaseAuth instance so it can be accessed globally.
public class FirebaseAuthManager : MonoBehaviour
{
    public static FirebaseAuth auth;

    void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
    }
}
