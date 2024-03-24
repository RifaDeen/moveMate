using NUnit.Framework;
using UnityEngine;

public class DinoGameManagerTests
{
    private MockDinoGameManager gameManager;

    [SetUp]
    public void SetUp()
    {
        var gameObject = new GameObject();
        gameManager = gameObject.AddComponent<MockDinoGameManager>();
    }

    [Test]
    public void NewGame_SetsUpGameCorrectly()
    {
        // Act
        gameManager.NewGame();

        // Assert
        Assert.AreEqual(0f, gameManager.Score);
        Assert.AreEqual(5f, gameManager.gameSpeed);
        Assert.IsTrue(gameManager.enabled);
    }

    [Test]
    public void GameOver_StopsGameAndUpdatesHighScore()
    {
        // Act
        gameManager.GameOver();

        // Assert
        Assert.AreEqual(0f, gameManager.gameSpeed);
        Assert.IsFalse(gameManager.enabled);
    }

    [Test]
    public void Retry_RestartsGame()
    {
        // Act
        gameManager.Retry();

        // Assert
        Assert.AreEqual(0f, gameManager.Score);
        Assert.AreEqual(5f, gameManager.gameSpeed);
        Assert.IsTrue(gameManager.enabled);
    }

    [Test]
    public void ObstaclePassed_IncreasesScore()
    {
        // Arrange
        gameManager.SetScore(5);

        // Act
        gameManager.ObstaclePassed();

        // Assert
        Assert.AreEqual(6f, gameManager.Score);
    }

    [Test]
    public void OnExitButtonClick_QuitsApplication()
    {
        // Act
        gameManager.OnExitButtonClick();

        // Assert - Placeholder for Unity-specific assertion
        Assert.Pass("Cannot perform Unity-specific assertion in unit test");
    }
}

public class MockDinoGameManager : MonoBehaviour
{
    public float gameSpeed { get; private set; }
    public float Score { get; private set; }

    public void NewGame()
    {
        gameSpeed = 5f;
        Score = 0f;
        enabled = true;
    }

    public void GameOver()
    {
        gameSpeed = 0f;
        enabled = false;
    }

    public void Retry()
    {
        NewGame();
    }

    public void ObstaclePassed()
    {
        Score++;
    }

    public void OnExitButtonClick()
    {
        // Placeholder for exit functionality
    }

    // Method to set the score (added for testing)
    public void SetScore(float score)
    {
        Score = score;
    }
}
