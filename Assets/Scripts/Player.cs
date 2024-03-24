using UnityEngine;
using UnityEngine.SceneManagement;
using Mediapipe.Unity.Sample.HandTracking;

public class BoxPlayer : MonoBehaviour
{
    public float moveSpeed;
    Rigidbody2D rb;
    BoxHandTrackingSolution handTracking;
    BoxGameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        handTracking = FindObjectOfType<BoxHandTrackingSolution>(); // Find the HandTrackingSolution component in the scene
        gameManager = FindObjectOfType<BoxGameManager>(); // Find the GameManager component in the scene
    }

    void Update()
    {
        // Use the boolean values from HandTrackingSolution to control the player movement
        if (handTracking.LeftTouching)
        {
            rb.AddForce(Vector2.left * moveSpeed);
        }
        else if (handTracking.RightTouching)
        {
            rb.AddForce(Vector2.right * moveSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            gameManager.GameOver(); // Call the GameManager's GameOver function
        }
    }
}
