using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;

public class RadialProgressTests : MonoBehaviour
{
    [UnityTest]
    public IEnumerator ProgressUpdatesOverTime()
    {
        // Arrange
        var gameObject = new GameObject();
        var radialProgress = gameObject.AddComponent<MockRadialProgress>();
        radialProgress.text = new MockText();
        radialProgress.image = new MockImage();
        radialProgress.speed = 10f;

        // Act
        yield return new WaitForSeconds(1f); // Wait for 1 second to allow progress to update

        // Assert
        Assert.GreaterOrEqual(radialProgress.image.GetCurrentFillAmount(), 0f);
        Assert.LessOrEqual(radialProgress.image.GetCurrentFillAmount(), 1f);
        Assert.GreaterOrEqual(int.Parse(radialProgress.text.text.Trim('%')), 0);
        Assert.LessOrEqual(int.Parse(radialProgress.text.text.Trim('%')), 100);
    }

    // Mock classes
    public class MockRadialProgress : MonoBehaviour
    {
        public Text text { get; set; }
        public MockImage image { get; set; }
        public float speed { get; set; }

        void Update()
        {
            if (image == null) return;

            if (image.GetCurrentFillAmount() < 100)
            {
                image.SetFillAmount(image.GetCurrentFillAmount() + speed * Time.deltaTime / 100f);
                text.text = ((int)image.GetCurrentFillAmount()) + "%";
            }
            else
            {
                text.text = "100%";
            }
        }
    }

    public class MockText : Text
    {
        public override string text { get; set; } = "50%"; // Set a default value for text
    }

    public class MockImage
    {
        private float fillAmount = 0.5f; // Initial fillAmount

        public float GetCurrentFillAmount()
        {
            return fillAmount;
        }

        public void SetFillAmount(float amount)
        {
            fillAmount = Mathf.Clamp01(amount);
        }
    }
}
