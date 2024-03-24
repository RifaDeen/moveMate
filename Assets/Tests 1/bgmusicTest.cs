using NUnit.Framework;

public class DinoBackgroundMusicTests
{
    [Test]
    public void PlayMusic_PlaysAudioIfNotPlaying()
    {
        // Arrange
        var backgroundMusic = new MockDinoBackgroundMusic();

        // Act
        backgroundMusic.PlayMusic();

        // Assert
        Assert.IsTrue(backgroundMusic.Played);
    }

    [Test]
    public void PlayMusic_DoesNotPlayAudioIfAlreadyPlaying()
    {
        // Arrange
        var backgroundMusic = new MockDinoBackgroundMusic();
        backgroundMusic.PlayMusic(); // Simulate already playing

        // Act
        backgroundMusic.PlayMusic();

        // Assert
        Assert.AreEqual(1, backgroundMusic.TimesAudioPlayed);
    }

    [Test]
    public void StopMusic_StopsAudioIfPlaying()
    {
        // Arrange
        var backgroundMusic = new MockDinoBackgroundMusic();
        backgroundMusic.PlayMusic(); // Simulate playing

        // Act
        backgroundMusic.StopMusic();

        // Assert
        Assert.IsTrue(backgroundMusic.Stopped);
    }

    [Test]
    public void StopMusic_DoesNotStopAudioIfNotPlaying()
    {
        // Arrange
        var backgroundMusic = new MockDinoBackgroundMusic();

        // Act
        backgroundMusic.StopMusic();

        // Assert
        Assert.AreEqual(0, backgroundMusic.TimesAudioStopped);
    }
}

public class MockDinoBackgroundMusic
{
    public bool Played { get; private set; }
    public bool Stopped { get; private set; }
    public int TimesAudioPlayed { get; private set; }
    public int TimesAudioStopped { get; private set; }

    public void PlayMusic()
    {
        Played = true;
        TimesAudioPlayed++;
    }

    public void StopMusic()
    {
        Stopped = true;
        TimesAudioStopped++;
    }
}
