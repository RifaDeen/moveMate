using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoronaObstacle : MonoBehaviour
{
    private GameObject player;
    private CoronaScoreManager scoreManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        scoreManager = FindObjectOfType<CoronaScoreManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Border")
        {
            Destroy(this.gameObject);
        }
        else if (collision.tag == "Player")
        {
            Destroy(player.gameObject);
            scoreManager.GameOver(); // Call the GameOver method in ScoreManager
        }
    }
}
