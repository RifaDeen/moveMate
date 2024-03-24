using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// the progress page 

public class GameProgress : MonoBehaviour
{
    [SerializeField] private TMP_Text progressText; //total game progress
    [SerializeField] private TMP_Text todayProgress; //today progress

    [SerializeField] private TMP_Text todayTime; //total time spent
    [SerializeField] private TMP_Text timeUnit;
    [SerializeField] TextMeshProUGUI levelText;
    private float gameprogressToday;

    RetrieveData retrieveData = new RetrieveData(); //retrieve data from the database
    private string userID;
    private User user;

    private string gameID;

    //radial progress bar
    [SerializeField] TextMeshProUGUI Radtext;
    [SerializeField] Image image;
    [SerializeField] float speed;

    void Start()
    {
        StartCoroutine(CalculategameProgress());
        StartCoroutine(calTodayProgress());
    }

    //overall game progress from all time and score 
    private IEnumerator CalculategameProgress()
    
    {
      
        if (AuthManager.CurrentUser != null)
        {
            userID = AuthManager.CurrentUser.UserId;
            Debug.Log("User ID obtained from user object: " + userID);
        }
        else
        {
            Debug.LogError("User object is null");
        }

        gameID = "gameid"; //gameid same for all games, to get the data of all games
        yield return StartCoroutine(retrieveData.RetrieveGameData(userID, gameID));

        List<int> scorelist = retrieveData.scoreList();
        List<float> timelist = retrieveData.timeList();

        float avgScore = calAvgScore(scorelist);
        Debug.Log("Average Score: " + avgScore);
        float avgTime = CalAvgTime(timelist);
        Debug.Log("Average Time: " + avgTime);

        float scorePercentage = (avgScore / 200) * 100;
        float timePercentage = (avgTime / 100) * 100;

        // Calculate the overall progress
        float gameprogress = (float)Math.Round((scorePercentage + timePercentage) / 2);

        progressText.text = gameprogress.ToString();

        calculateRank(gameprogress);
    }

    public IEnumerator calTodayProgress()
    {
        gameprogressToday = 0;

      
        if (AuthManager.CurrentUser != null)
        {
            userID = AuthManager.CurrentUser.UserId;
            Debug.Log("User ID obtained from user object: " + userID);
        }
        else
        {
            Debug.LogError("User object is null");
        }
        
        gameID = "gameid";
        DateTime utcNow = DateTime.UtcNow;
        DateTime todayUtc = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 0, 0, 0, DateTimeKind.Utc);

        yield return StartCoroutine(retrieveData.RetrieveGameDataByDate(userID, gameID, todayUtc));
        List<int> scorelist = retrieveData.ScoreListToday();
        List<float> timelist = retrieveData.TimeListToday();

        Debug.Log("Score List tot: " + calScore(scorelist));

        float avgScore = calAvgScore(scorelist);
        Debug.Log("Average Score: " + avgScore);
        float avgTime = CalAvgTime(timelist);
        Debug.Log("Average Time: " + avgTime);
        float totTimeinSec = CalTime(timelist);

        //default goal for user
        //for score = 150 points per day
        //time = 1800s / half an hour per day
        float scorePercentage = (avgScore / 150) * 100;
        float timePercentage = (avgTime / 1800) * 100;

        // Calculate the overall progress of today
        gameprogressToday = (float)Math.Round((scorePercentage + timePercentage) / 2);

        todayProgress.text = gameprogressToday.ToString();
        todayTime.text = FormatTime(totTimeinSec);


        if (gameprogressToday == 0)
        {
            Radtext.text = "0%";
        }
        else if (gameprogressToday < 100)
        {
            gameprogressToday += speed * Time.deltaTime;
            Radtext.text = ((int)gameprogressToday).ToString() + "%";
        }
        else
        {
            Radtext.text = "100%";
        }
        image.fillAmount = gameprogressToday / 100;

    }

    //function that calculates average score 

    public float calAvgScore(List<int> list)
    {
        int sum = 0;
        foreach (int score in list)
        {
            sum += score;
        }
        return (float)Math.Round((float)sum / list.Count, 1);
    }

    //function that calculates average time
    public float CalAvgTime(List<float> list)
    {
        int sum = 0;
        foreach (int time in list)
        {
            sum += time;
        }
        return (float)Math.Round((float)sum / list.Count, 1);
    }


    //calculate total time spent
    public float CalTime(List<float> list)
    {
        float totalTime = 0;

        foreach (var time in list)
        {
            totalTime += time;
        }

        return totalTime;
    }

    //calculate total score

    public int calScore(List<int> list)
    {
        int totScore = 0;

        foreach (var score in list)
        {
            totScore += score;
        }

        return totScore;
    }

    //ui formatting for time
    private string FormatTime(float totalSeconds)
    {
        if (totalSeconds < 60)
        {
            timeUnit.text = "sec";
            return Math.Round(totalSeconds).ToString();
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

    private void calculateRank(float progress)
    {
        if (progress < 20)
        {
            levelText.text = "Rookie";
        }
        else if (progress < 40)
        {
            levelText.text = "Amateur";
        }
        else if (progress < 60)
        {
            levelText.text = "Intermediate";
        }
        else if (progress < 80)
        {
            levelText.text = "Advanced";
        } 
        else if (progress < 90)
        {
            levelText.text = "Expert";
        }
        else
        {
            levelText.text = "Master";
        }
    }

}