using UnityEngine;

public class SqueebEnemy : MonoBehaviour
{
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f; // Distance to patrol from starting point
    public Animator animator;
    
    [Header("SFX")]
    [SerializeField] private AudioClip squeebSound;

    private Vector2 startPosition;
    private Vector2 leftLimit;
    private Vector2 rightLimit;
    private Vector2 targetPosition;
    private bool isDead = false;

    private void Start()
    {
        startPosition = transform.position;
        leftLimit = startPosition + Vector2.left * patrolDistance;
        rightLimit = startPosition + Vector2.right * patrolDistance;
        targetPosition = rightLimit; // Start by moving to the right limit
    }

    private void Update()
    {
        if (isDead) return;

        Patrol();
    }

    private void Patrol()
    {
        // Move towards the target position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);

        // Check if reached the target position
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Switch to the opposite patrol limit
            targetPosition = targetPosition == leftLimit ? rightLimit : leftLimit;
            Flip(); // Flip the enemy's direction
        }
    }


    private void Flip()
    {
        // Flip the enemy to face the other direction
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            Die();
        }
        // Check if the player collided with the top of the enemy
        if (other.gameObject.CompareTag("Player"))
        {
            // Get collision contact points
            ContactPoint2D[] contacts = other.contacts;
            foreach (var contact in contacts)
            {
                // Check if the player is above the enemy
                if (contact.normal.y <= -0.5f)
                {
                    
                    Die();
                    break;
                }
                
                other.gameObject.GetComponent<Health>()?.TakeDamage(1);
                
            }
        }
    }

    public void Die()
    {
        SoundManager.instance.PlaySound(squeebSound);
        isDead = true;
        animator.SetTrigger("Die"); // Play death animation
        Destroy(gameObject, 0.5f); // Destroy enemy after a short delay
    }
}
