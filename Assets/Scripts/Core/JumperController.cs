using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumperController : MonoBehaviour
{
    public float baseSpeed = 2f; // Base speed of the camera moving upwards
    public float speedBoost = 1f; // Additional speed when the player is in the top zone
    public Transform player; // Reference to the player’s transform
    public float topZoneDistance = 5f; // Distance from the camera where it starts following the player in the top zone
    public float maxHeight = 20f; // Maximum height limit for the camera
    public float detectionThreshold = -5f; // Distance below camera to consider player stuck
    public float boostForce = 5f; // Amount of force to push the player upwards
    public float disableCollisionDuration = 0.5f; // How long to disable collision
    public GameObject platforms;
    public GameObject platforms2;
    private float currentSpeed;
    private bool allIsWell = true;
    private bool isStuck;

    private Rigidbody2D rb;
    private Collider2D playerCollider;

    private void Start()
    {
        currentSpeed = baseSpeed;
        rb = player.GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (allIsWell)
        {
            CheckPlayerPosition();
            PanCamera();

            if (Input.GetKeyDown(KeyCode.Space))
            {
               // StartCoroutine(turnOffPlatformes());
            }
        }
    }

    private void OnEnable()
    {
        Health.OnPlayerDied += OnPlayerDiedHandler;
        MiniGameController.OnLevelComplete += OnPlayerDiedHandler;
    }

    private void OnDisable()
    {
        Health.OnPlayerDied -= OnPlayerDiedHandler;
        MiniGameController.OnLevelComplete -= OnPlayerDiedHandler;
    }

    private void OnPlayerDiedHandler()
    {
        allIsWell = false;
    }

    private void PanCamera()
    {
        // Check if the camera's Y-position is below the maxHeight limit
        if (transform.position.y < maxHeight)
        {
            // Always move the camera upward at the base speed
            transform.position += Vector3.up * baseSpeed * Time.deltaTime;

            // If the player is in the top zone, smoothly adjust the camera position to follow them
            if (player.position.y >= transform.position.y + topZoneDistance)
            {
                // Smoothly move the camera towards the player's position within the top zone
                float targetY = Mathf.Min(player.position.y - topZoneDistance, maxHeight);
                transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * speedBoost), transform.position.z);
            }
        }

        // Check if the player has fallen below the camera's view threshold
        if (player.position.y < transform.position.y + detectionThreshold && !isStuck)
        {
            // If the player is stuck below, start the unsticking process
            StartCoroutine(UnstickPlayer());
        }
    }

    private void CheckPlayerPosition()
    {
        // Adjust current speed to include boost only if the player is actively in the top zone
        if (player.position.y >= transform.position.y)
        {
            currentSpeed = baseSpeed + speedBoost;
        }
        else
        {
            currentSpeed = baseSpeed;
        }
    }

    private IEnumerator UnstickPlayer()
    {
        isStuck = true;

        // Turn off player collision
        playerCollider.enabled = false;

        // Apply a vertical boost to bring the player back into view
        rb.AddForce(Vector2.up * boostForce, ForceMode2D.Impulse);

        // Wait for the specified duration
        yield return new WaitForSeconds(disableCollisionDuration);

        // Re-enable collision after boost
        playerCollider.enabled = true;

        isStuck = false;
    }

    private IEnumerator turnOffPlatformes()
    {
        platforms.GetComponent<Collider2D>().enabled = false;
        platforms2.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(.5f);
        platforms.GetComponent<Collider2D>().enabled = true;
        platforms2.GetComponent<Collider2D>().enabled = true;
    }
}
