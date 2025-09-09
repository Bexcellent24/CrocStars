using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Minimum and maximum positions for clamping
    [SerializeField] private float minY; // Bottom clamp (to avoid empty space below)
    [SerializeField] private float maxY; // Top clamp
    [SerializeField] private float minX; // Left clamp
    [SerializeField] private float maxX; // Right clamp

    // Follow player
    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    [SerializeField] private float yOffset = 2f; // Offset to keep the camera above the player
    private float lookAhead;

    private void Update()
    {
        // Follow player with clamping on X and Y axes
        float targetX = Mathf.Clamp(player.position.x + lookAhead, minX, maxX); // Clamp X position (left and right)
        
        // Add yOffset to targetY to keep the camera slightly above the player
        float targetY = Mathf.Clamp(player.position.y + yOffset, minY, maxY);   // Clamp Y position (top and bottom)

        // Set the camera position
        transform.position = new Vector3(targetX, targetY, transform.position.z);

        // Smoothly adjust the lookAhead based on player movement
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
    }
}