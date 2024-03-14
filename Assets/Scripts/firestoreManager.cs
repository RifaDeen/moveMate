using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class FirestoreManager
{
    FirebaseFirestore db;

    public FirestoreManager()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    public void StoreUserData(User user)
    {
        // Assuming you want to store user data in a collection named "users"
        CollectionReference usersCollection = db.Collection("YOUR_COLLECTION_NAME");

        // Convert the User object to a dictionary
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "userId", user.UserId },
            { "username", user.Username },
            { "email", user.Email }
            // Add more fields if needed
        };

        // Add the user data to the Firestore collection
        usersCollection.AddAsync(userData)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"Failed to store user data in Firestore: {task.Exception}");
                }
                else
                {
                    Debug.Log("User data stored successfully in Firestore.");
                }
            });
    }
}
