using System;
using System.Collections.Generic;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class GameUtils
{
    public string GenerateGameInstanceId()
    {
        long timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        System.Random random = new System.Random();
        int randomComponent = random.Next(10000);
        string gameInstanceId = $"{timestamp}-{randomComponent}";
        return gameInstanceId;
    }

    public void SaveGameDataToFirestore(string userId, string gameId, string gameInstanceId, int score, float time)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        DocumentReference gameHistoryRef = db.Collection("users").Document(userId)
            .Collection("game_history").Document(gameId).Collection("game_instances").Document(gameInstanceId);

        Dictionary<string, object> gameData = new Dictionary<string, object>
        {
            { "score", score },
            {"Time", time},
            { "date", DateTime.UtcNow } // Store the current date
        };

        gameHistoryRef.SetAsync(gameData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to write game data to Firestore: " + task.Exception);
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("Firestore write operation was cancelled");
            }
            else
            {
                Debug.Log("Game data successfully written to Firestore");
            }
        });
    }
}