using System.Collections;
using UnityEngine;
using UnityEngineInternal;

public class RunnerController : MonoBehaviour
{
    public float baseSpeed = 2f; // Base speed of the camera
    public float speedBoost = 1f; // Additional speed when the player is in the front half
    public Transform player; // Reference to the player’s transform
    public float frontZoneDistance = 5f; // Distance from the camera where it follows the player in the front zone
    public float detectionThreshold = -5f; // Distance behind camera to consider player stuck
    public float boostForce = 5f; // Amount of force to push the player forward
    public float disableCollisionDuration = 0.5f; // How long to disable collision

    private float currentSpeed;
    private bool allIsWell = true;
    private bool isStuck;

    public Rigidbody2D rb;
    private Collider2D playerCollider;

    private void Start()
    {
        currentSpeed = baseSpeed;
            // rb = player.GetComponent<Rigidbody2D>();
        Debug.Log(rb);
        playerCollider = player.GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (allIsWell)
        {
            CheckPlayerPosition();
            PanCamera();
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
        // Always move the camera forward at least at the base speed
        transform.position += Vector3.right * baseSpeed * Time.deltaTime;

        // If player is in the front zone, adjust camera position to follow them
        if (player.position.x >= transform.position.x + frontZoneDistance)
        {
            float targetX = player.position.x - frontZoneDistance;
            transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * speedBoost), transform.position.y, transform.position.z);
        }

        // Check if the player is behind the camera's view threshold
        if (player.position.x < transform.position.x + detectionThreshold && !isStuck)
        {
            // Start the unsticking process if the player is too far behind
            StartCoroutine(UnstickPlayer());
        }
    }

    private void CheckPlayerPosition()
    {
        // Adjust the current speed based on player position, if necessary
        if (player.position.x >= transform.position.x)
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

        // Temporarily disable player collision
        player.GetComponent<PlayerMovement>().enabled = false;
        rb.gravityScale = 0;
        playerCollider.enabled = false;
        

        //Vector2 boostDirection = new Vector2(boostForce * 2f, boostForce);
        rb.AddForce(Vector2.right * boostForce, ForceMode2D.Impulse);
        
        //rb.velocity = new Vector2(boostForce * 3f, boostForce);
        
        //rb.MovePosition(rb.position + new Vector2(boostForce, 0) * Time.deltaTime);
        
        yield return new WaitForSeconds(disableCollisionDuration);
        
        // Re-enable collision after boost
        playerCollider.enabled = true;
        rb.gravityScale = 7;
        player.GetComponent<PlayerMovement>().enabled = true;
        
        isStuck = false;
    }
}
