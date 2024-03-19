using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RetrieveData {

    //stroring dictionary in a dictionary -  { "flappy_bird" : [(score, time)], "dino_game" : [(score, time)]}
    Dictionary<string, List<(int score, float time)>> gameDataDictionary = new Dictionary<string, List<(int score, float time)>>();

    // // public async void RetrieveGameDataFromFirestore(string userId, string gameId)
    // {
    //     FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

    //     CollectionReference gameInstancesRef = db.Collection("users").Document(userId)
    //         .Collection("game_history").Document(gameId).Collection("game_instances");

    //     await gameInstancesRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
    //     {
    //         if (task.IsFaulted)
    //         {
    //             UnityEngine.Debug.Log("Failed to retrieve game data from Firestore: " + task.Exception);
    //             return;
    //         }

    //         List<(int score, float time)> scoreTimeDict = new List<(int score, float time)>();

    //         foreach (DocumentSnapshot document in task.Result.Documents)
    //         {

    //             Dictionary<string, object> gameData = document.ToDictionary();
    //             /* checks if the dictionary contains keys "score" and "Time" 
    //             If both keys are found, it retrieves their corresponding values 
    //             and assigns them to variables score and time respectively */
    //             if (gameData.TryGetValue("score", out object scoreObj) && gameData.TryGetValue("Time", out object timeObj))
    //             {
    //                 int score = Convert.ToInt32(scoreObj); //converts to appropriate data type
    //                 float time = Convert.ToSingle(timeObj);
    //                 scoreTimeDict.Add((score, time));
    //                 Debug.Log("Score: " + score + ", Time: " + time);

    //             }

    //         }

    //         Debug.Log("Number of documents retrieved: " + task.Result.Documents.Count());
    //         gameDataDictionary[gameId] = scoreTimeDict;
    //     });

    //     PrintList(scoreList());
    //     PrintGameDataDictionary();

    // }

    public IEnumerator RetrieveGameDataFromFirestore(string userId, string gameId)
{
    FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

    CollectionReference gameInstancesRef = db.Collection("users").Document(userId)
        .Collection("game_history").Document(gameId).Collection("game_instances");

    var task = gameInstancesRef.GetSnapshotAsync();

    yield return new WaitUntil(() => task.IsCompleted);

    if (task.IsFaulted)
    {
        UnityEngine.Debug.Log("Failed to retrieve game data from Firestore: " + task.Exception);
        yield break;
    }

    List<(int score, float time)> scoreTimeDict = new List<(int score, float time)>();

    foreach (DocumentSnapshot document in task.Result.Documents)
    {
        Dictionary<string, object> gameData = document.ToDictionary();

        if (gameData.TryGetValue("score", out object scoreObj) && gameData.TryGetValue("Time", out object timeObj))
        {
            int score = Convert.ToInt32(scoreObj);
            float time = Convert.ToSingle(timeObj);
            scoreTimeDict.Add((score, time));
            Debug.Log("Score: " + score + ", Time: " + time);
        }
    }

    Debug.Log("Number of documents retrieved: " + task.Result.Documents.Count());
    gameDataDictionary[gameId] = scoreTimeDict;
}


    //calculate total score for all games
     public int CalculateTotalScore()
    {
        Debug.Log("task 2");
        int totalScore = 0;
        int scoreCount =0;
        foreach (var scoreTimeDict in gameDataDictionary.Values)
        {   
            UnityEngine.Debug.Log("score and time " + scoreTimeDict);
            foreach (var scoreVal in scoreTimeDict)
            {   
                UnityEngine.Debug.Log("Score: " + scoreVal.score);
                totalScore += scoreVal.score;
                scoreCount++;
            }
        }

        UnityEngine.Debug.Log("Total Score: " + totalScore + ", Score Count: " + scoreCount);
        return totalScore;

    }

    //cal total time for all
    public float CalculateTotalTime()
    {
        float totalTime = 0;
        int timeCount = 0;
        foreach (var scoreTimeDict in gameDataDictionary.Values)
        {
            foreach (var timeValue in scoreTimeDict)
            {
                totalTime += timeValue.time;
                timeCount++;
            }
        }

        UnityEngine. Debug.Log("Total Time: " + totalTime + ", Time Count: " + timeCount);
        return totalTime;
    }

    public void PrintGameDataDictionary()
    {
        
        UnityEngine.Debug.Log("print function running");
        foreach (var gameData in gameDataDictionary)
        {
        string gameId = gameData.Key;
        var scoreTimeList = gameData.Value;

        foreach (var st in scoreTimeList)
        {
            UnityEngine.Debug.Log($"Game ID: {gameId}, Score: {st.score}, Time: {st.time}");
        }
        }
    }

    public List<int> scoreList(){
        List<int> listscore = new List<int>();

        foreach (var scoreTimeDict in gameDataDictionary.Values)
        {   
            foreach (var scoreVal in scoreTimeDict)
            {   
               listscore.Add(scoreVal.score);
            }
        }
        return listscore;
    }



     public List<float> timeList(){

        List<float> listtime = new List<float>();

        foreach (var scoreTimeDict in gameDataDictionary.Values)
        {   
            foreach (var timeValue in scoreTimeDict)
            {
               listtime.Add(timeValue.time);
            }
        }
        return listtime;
    }

    public void PrintList(List<int> list)
{
    foreach (int item in list)
    {
        Debug.Log("score is " + item);
    }
}
}
