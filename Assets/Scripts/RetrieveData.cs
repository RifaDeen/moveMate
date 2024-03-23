using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RetrieveData
{

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

    public IEnumerator RetrieveGameData(string userId, string gameId)
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
    Dictionary<string, List<(int score, float time, DateTime date)>> gameDataDictionaryWithDate = new Dictionary<string, List<(int score, float time, DateTime date)>>();
    public IEnumerator RetrieveGameDataByDate(string userId, string gameId, DateTime specificDate)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        CollectionReference gameInstancesRef = db.Collection("users").Document(userId)
            .Collection("game_history").Document(gameId).Collection("game_instances");

        Query gameInstancesQuery = gameInstancesRef.WhereEqualTo("date", specificDate.Date);

        var task = gameInstancesQuery.GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            UnityEngine.Debug.Log("Failed to retrieve game data from Firestore: " + task.Exception);
            yield break;
        }

        List<(int score, float time, DateTime date)> scoreTimeDateList = new List<(int score, float time, DateTime date)>();

        foreach (DocumentSnapshot document in task.Result.Documents)
        {
            Dictionary<string, object> gameData = document.ToDictionary();

            if (gameData.TryGetValue("score", out object scoreObj) && gameData.TryGetValue("Time", out object timeObj) && gameData.TryGetValue("date", out object dateObj))
            {
                int score = Convert.ToInt32(scoreObj);
                float time = Convert.ToSingle(timeObj);
                DateTime date = (DateTime)dateObj;

                scoreTimeDateList.Add((score, time, date));
                Debug.Log("Score: " + score + ", Time: " + time + ", Date: " + date);
            }
        }

        Debug.Log("Number of documents retrieved: " + task.Result.Documents.Count());
        gameDataDictionaryWithDate[gameId] = scoreTimeDateList;
    }

    //print list of scores for debugging purposes
    public List<int> scoreList()
    {
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


    //print list of time for debugging purposes
    public List<float> timeList()
    {

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
    public List<int> ScoreListToday()
    {
        List<int> listscore = new List<int>();

        foreach (var gameData in gameDataDictionaryWithDate.Values)
        {
            foreach (var scoreTimeDate in gameData)
            {
                listscore.Add(scoreTimeDate.score);
            }
        }
        return listscore;
    }



    public List<float> TimeListToday()
    {

        List<float> listtime = new List<float>();

        foreach (var gameData in gameDataDictionaryWithDate.Values)
        {
            foreach (var scoreTimeDate in gameData)
            {
                listtime.Add(scoreTimeDate.time);
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
}
