using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameProgress : MonoBehaviour
{
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text todayProgress;

    [SerializeField] private TMP_Text todayTime;
    [SerializeField] private TMP_Text timeUnit;


    private int totalProgress; // The total progress needed to reach 100%
    private int currentProgress; // The player's current progress

    RetrieveData retrieveData = new RetrieveData();
    private string userID;

    private string gameID;

    void Start()
    {
        totalProgress = 100;
        currentProgress = 0;
        StartCoroutine(CalculategameProgress());
        StartCoroutine(calTodayProgress());
    }

        private IEnumerator CalculategameProgress()
    {
        userID = "newplayer";
        gameID = "gameid";
        yield return StartCoroutine(retrieveData.RetrieveGameData(userID, gameID));

        List<int> scorelist = retrieveData.scoreList();
        List<float> timelist = retrieveData.timeList();

        float avgScore = calAvgScore(scorelist);
        Debug.Log("Average Score: " + avgScore);
        float avgTime = calAvgTime(timelist);
        Debug.Log("Average Time: " + avgTime);

        // Normalize the average score and time to a scale of 0 to 100
        float scorePercentage = (avgScore / 200) * 100;
        float timePercentage = (avgTime / 50) * 100;

        // Calculate the overall progress
        float gameprogress = (float)Math.Round((scorePercentage + timePercentage) / 2);

        progressText.text = gameprogress.ToString();
    }

    private IEnumerator calTodayProgress(){ 
        userID = "newplayer";
        gameID = "gameid";
        DateTime today = DateTime.Today;

        yield return StartCoroutine(retrieveData.RetrieveGameDataByDate(userID, gameID,today));
        List<int> scorelist = retrieveData.scoreListToday();
        List<float> timelist = retrieveData.timeListToday();

        Debug.Log("Score List tot: " + calScore(scorelist));

        float avgScore = calAvgScore(scorelist);
        Debug.Log("Average Score: " + avgScore);
        float totTimeinSec = CalTime(timelist);

        // Normalize the average score and time to a scale of 0 to 100
        float scorePercentage = (avgScore / 200) * 100;

        todayProgress.text = Math.Round(scorePercentage).ToString();
        todayTime.text = FormatTime(totTimeinSec);

    }

    public float calAvgScore(List<int> list)
    {
        int sum = 0;
        foreach (int score in list)
        {
            sum += score;
        }
        return (float)Math.Round((float)sum / list.Count, 1);
    }

    public float calAvgTime(List<float> list)
    {
        int sum = 0;
        foreach (int time in list)
        {
            sum += time;
        }
        return (float)Math.Round((float)sum / list.Count, 1);
    }

    public float CalTime(List<float> list)
    {
        float totalTime = 0;

        foreach (var time in list)
        {
            totalTime += time;
        }

        return totalTime;
    }

        public int calScore(List<int> list)
    {
        int totScore = 0;

        foreach (var score in list)
        {
            totScore += score;
        }

        return totScore;
    }

    public string FormatTime(float totalSeconds)
    {
        if (totalSeconds < 60)
        {
            timeUnit.text = "sec";
            return  Math.Round(totalSeconds).ToString();
        }
        else if (totalSeconds < 3600) // 60 minutes * 60 seconds
        {
            timeUnit.text = "min";
            return Math.Round(totalSeconds / 60).ToString();
        }
        else
        {
            timeUnit.text = "hr";
            return Math.Round(totalSeconds / 3600).ToString(); // 60 minutes * 60 seconds
        }
    }
    
}