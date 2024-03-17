using Mediapipe.Unity.Sample.HandTracking;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DinoPlayer : MonoBehaviour
{
    private CharacterController character;
    private Vector3 direction;

    public float jumpForce = 8f;
    public float gravity = 9.81f * 2f;

    private void Awake()
    {
        character = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        direction = Vector3.zero;
    }

    private void Update()
    {
        direction += gravity * Time.deltaTime * Vector3.down;

        if (character.isGrounded)
        {
            direction = Vector3.down;

            dinoHandTrackingSolution handTrackingSolution = FindObjectOfType<dinoHandTrackingSolution>();
            if (handTrackingSolution != null && handTrackingSolution.IsHandClosed) {
                direction = Vector3.up * jumpForce;
            }
        }

        character.Move(direction * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle")) {
            FindObjectOfType<dinoGameManager>().GameOver();
        }
    }

}