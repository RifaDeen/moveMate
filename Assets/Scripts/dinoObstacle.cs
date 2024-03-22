using UnityEngine;

public class dinoObstacle : MonoBehaviour
{
    private float leftEdge;
    private bool isPassed = false;

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 2f;
    }

    private void Update()
    {
        transform.position += dinoGameManager.Instance.gameSpeed * Time.deltaTime * Vector3.left;

        if (!isPassed && transform.position.x < leftEdge)
        {
            // Notify GameManager that obstacle is passed
            dinoGameManager.Instance.ObstaclePassed();
            isPassed = true; // Mark obstacle as passed to avoid multiple notifications
            Destroy(gameObject);
        }
    }
}
