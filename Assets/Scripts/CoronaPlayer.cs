using System.Collections;
using System.Collections.Generic;
using Mediapipe.Unity.Sample.HandTracking;
using UnityEngine;

public class CoronaPlayer : MonoBehaviour
{
    public float playerSpeed;
    private Rigidbody2D rb;
    private CoronaHandTrackingSolution handTrackingSolution; // Reference to HandTrackingSolution script

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        handTrackingSolution = FindObjectOfType<CoronaHandTrackingSolution>(); // Find the HandTrackingSolution script in the scene
    }

    // Update is called once per frame
    void Update()
    {
        // Use isExpanded value from HandTrackingSolution script to determine player movement
        if (handTrackingSolution.isExpanded)
        {
            // Move up in the y direction
            rb.velocity = new Vector2(0, playerSpeed);
        }
        else
        {
            // Move down in the y direction
            rb.velocity = new Vector2(0, -playerSpeed);
        }
    }
}
