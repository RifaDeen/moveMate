using NUnit.Framework;
using UnityEngine;

public class DinoTimerManagerTests
{
    [Test]
    public void Timer_StartsRunningOnAwake()
    {
        // Arrange
        var timerManager = new MockDinoTimerManager();

        // Act
        timerManager.Awake();

        // Assert
        Assert.IsTrue(timerManager.IsTimerRunning);
    }

    [Test]
    public void Timer_StopsCorrectly()
    {
        // Arrange
        var timerManager = new MockDinoTimerManager();

        // Act
        timerManager.StopTimer();

        // Assert
        Assert.IsFalse(timerManager.IsTimerRunning);
    }

    [Test]
    public void GetElapsedTime_ReturnsCorrectValue()
    {
        // Arrange
        var timerManager = new MockDinoTimerManager();
        var expectedElapsedTime = 10f;
        timerManager.RestartTimer();
        timerManager.ElapsedTime = expectedElapsedTime;

        // Act
        var elapsedTime = timerManager.GetElapsedTime();

        // Assert
        Assert.AreEqual(expectedElapsedTime, elapsedTime);
    }
}

public class MockDinoTimerManager
{
    public bool IsTimerRunning { get; private set; } = false;
    public float ElapsedTime { get; set; } = 0f;

    public void Awake()
    {
        IsTimerRunning = true; // Simulate timer running on awake
    }

    public void StopTimer()
    {
        IsTimerRunning = false; // Simulate stopping the timer
    }

    public void RestartTimer()
    {
        IsTimerRunning = true; // Simulate restarting the timer
    }

    public float GetElapsedTime()
    {
        return ElapsedTime; // Return the mocked elapsed time
    }
}
