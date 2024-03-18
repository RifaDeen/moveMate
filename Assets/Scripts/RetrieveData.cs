using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;

public class RetreiveData {
public void RetrieveGameDataFromFirestore(string userId, string gameId)
{
    FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

    CollectionReference gameInstancesRef = db.Collection("users").Document(userId)
        .Collection("game_history").Document(gameId).Collection("game_instances");

    gameInstancesRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
        {
            Console.WriteLine("Failed to retrieve game data from Firestore: " + task.Exception);
            return;
        }

        List<int> scores = new List<int>();
        List<float> times = new List<float>();

        foreach (DocumentSnapshot document in task.Result.Documents)
        {
            Dictionary<string, object> gameData = document.ToDictionary();
            /* checks if the dictionary contains keys "score" and "Time" 
            If both keys are found, it retrieves their corresponding values 
            and assigns them to variables score and time respectively */
            if (gameData.TryGetValue("score", out object score) && gameData.TryGetValue("Time", out object time))
            {
                scores.Add(Convert.ToInt32(score)); //converts to apprpirate data type
                times.Add(Convert.ToSingle(time));
            }
        }

    });
}
}
