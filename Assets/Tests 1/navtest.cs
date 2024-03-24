using UnityEngine;
using UnityEngine.SceneManagement;
using NUnit.Framework;

public interface ISceneLoader
{
    void LoadScene(string sceneName);
}

public class SceneManagerWrapper : ISceneLoader
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

public class MockSceneLoader : ISceneLoader
{
    public string LoadedScene { get; private set; }

    public void LoadScene(string sceneName)
    {
        LoadedScene = sceneName;
    }
}

public class navigation : MonoBehaviour
{
    private ISceneLoader sceneLoader;

    public navigation(ISceneLoader loader)
    {
        sceneLoader = loader;
    }

    public void ChangeScene(string sceneName)
    {
        sceneLoader.LoadScene(sceneName);
    }
}

public class NavigationTests
{
    [Test]
    public void ChangeScene_LoadsCorrectScene()
    {
        // Arrange
        var mockLoader = new MockSceneLoader();
        var nav = new navigation(mockLoader);
        string sceneName = "TestScene";

        // Act
        nav.ChangeScene(sceneName);

        // Assert
        Assert.AreEqual(sceneName, mockLoader.LoadedScene);
    }
}
