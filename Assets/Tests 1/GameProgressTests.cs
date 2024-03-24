using NUnit.Framework;
using System;
using System.Collections.Generic;

public class GameProgressTests
{
    [Test]
    public void CalAvgScore_ReturnsCorrectAverage()
    {
        // Arrange
        var gameProgress = new MockGameProgress();
        var scores = new List<int> { 80, 90, 70 };

        // Act
        float averageScore = gameProgress.calAvgScore(scores);

        // Assert
        Assert.AreEqual(80f, averageScore); // Expected average score is (80 + 90 + 70) / 3 = 80
    }

    [Test]
    public void CalAvgTime_ReturnsCorrectAverage()
    {
        // Arrange
        var gameProgress = new MockGameProgress();
        var times = new List<float> { 30f, 45f, 60f };

        // Act
        float averageTime = gameProgress.CalAvgTime(times);

        // Assert
        Assert.AreEqual(45f, averageTime); // Expected average time is (30 + 45 + 60) / 3 = 45
    }

    [Test]
    public void CalTime_ReturnsCorrectTotalTime()
    {
        // Arrange
        var gameProgress = new MockGameProgress();
        var times = new List<float> { 30f, 45f, 60f };

        // Act
        float totalTime = gameProgress.CalTime(times);

        // Assert
        Assert.AreEqual(135f, totalTime); // Expected total time is 30 + 45 + 60 = 135
    }

    [Test]
    public void CalScore_ReturnsCorrectTotalScore()
    {
        // Arrange
        var gameProgress = new MockGameProgress();
        var scores = new List<int> { 80, 90, 70 };

        // Act
        int totalScore = gameProgress.calScore(scores);

        // Assert
        Assert.AreEqual(240, totalScore); // Expected total score is 80 + 90 + 70 = 240
    }

    [Test]
    public void FormatTime_ReturnsFormattedString()
    {
        // Arrange
        var gameProgress = new MockGameProgress();
        float totalSeconds = 125;

        // Act
        string formattedTime = gameProgress.FormatTime(totalSeconds);

        // Assert
        Assert.AreEqual("2 min", formattedTime); // 125 seconds should be formatted as "2 min"
    }

    [Test]
    public void CalculateRank_ReturnsCorrectRank()
    {
        // Arrange
        var gameProgress = new MockGameProgress();

        // Act & Assert
        Assert.AreEqual("Rookie", gameProgress.calculateRank(10)); // Progress < 20, should be "Rookie"
        Assert.AreEqual("Amateur", gameProgress.calculateRank(30)); // Progress < 40, should be "Amateur"
        Assert.AreEqual("Intermediate", gameProgress.calculateRank(50)); // Progress < 60, should be "Intermediate"
        Assert.AreEqual("Advanced", gameProgress.calculateRank(70)); // Progress < 80, should be "Advanced"
        Assert.AreEqual("Expert", gameProgress.calculateRank(85)); // Progress < 90, should be "Expert"
        Assert.AreEqual("Master", gameProgress.calculateRank(95)); // Progress >= 90, should be "Master"
    }

    // MockGameProgress class to mock GameProgress
    public class MockGameProgress
    {
        public float calAvgScore(List<int> list)
        {
            // Simulate the behavior of calAvgScore method
            int sum = 0;
            foreach (int score in list)
            {
                sum += score;
            }
            return (float)Math.Round((float)sum / list.Count, 1);
        }

        public float CalAvgTime(List<float> list)
        {
            // Simulate the behavior of CalAvgTime method
            int sum = 0;
            foreach (int time in list)
            {
                sum += time;
            }
            return (float)Math.Round((float)sum / list.Count, 1);
        }

        public float CalTime(List<float> list)
        {
            // Simulate the behavior of CalTime method
            float totalTime = 0;
            foreach (float time in list)
            {
                totalTime += time;
            }
            return totalTime;
        }

        public int calScore(List<int> list)
        {
            // Simulate the behavior of calScore method
            int totScore = 0;
            foreach (int score in list)
            {
                totScore += score;
            }
            return totScore;
        }

                public string FormatTime(float totalSeconds)
        {
            // Simulate the behavior of FormatTime method
            if (totalSeconds < 60)
            {
                return Math.Round(totalSeconds).ToString() + " sec";
            }
            else if (totalSeconds < 3600) // 60 minutes * 60 seconds
            {
                return Math.Round(totalSeconds / 60).ToString() + " min";
            }
            else
            {
                return Math.Round(totalSeconds / 3600).ToString() + " hr"; // 60 minutes * 60 seconds
            }
        }

        public string calculateRank(float progress)
        {
            // Simulate the behavior of calculateRank method
            if (progress < 20)
            {
                return "Rookie";
            }
            else if (progress < 40)
            {
                return "Amateur";
            }
            else if (progress < 60)
            {
                return "Intermediate";
            }
            else if (progress < 80)
            {
                return "Advanced";
            }
            else if (progress < 90)
            {
                return "Expert";
            }
            else
            {
                return "Master";
            }
        }
    }
}

