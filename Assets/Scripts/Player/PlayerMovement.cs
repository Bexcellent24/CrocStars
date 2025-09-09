using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float baseSpeed = 5f; // Base speed
    [SerializeField] private float baseJumpPower = 10f; // Base jump power
    private float speed;
    private float jumpPower;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX;
    [SerializeField] private float wallJumpY;
    
    [Header("Dash Parameters")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    private bool isDashing = false;
    private Vector2 dashDirection; 

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip dashSound;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float horizontalInput;
    private Vector3 originalScale;

    public PlayerInventory inventory;
    private JibbitManager jibbitManager;
    private bool isInAir; // Track if the player is in the air
    private bool isOnWall;
    private bool wasGrounded = true; // Track if player was grounded last frame
    private int jumpCounter = 1;
    
    private void Awake()
    {
        // Use the inventory from GameManager
        inventory = GameManager.instance.playerInventory; // Get inventory from GameManager
        
        originalScale = transform.localScale;
        jibbitManager = GetComponent<JibbitManager>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        
        // Initialize speed and jump power with base values
        speed = baseSpeed;
        jumpPower = baseJumpPower;
        
    }
    
    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0.01f)
            transform.localScale = new Vector3(originalScale.x, originalScale.y, originalScale.z);
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-originalScale.x, originalScale.y, originalScale.z);

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());
        
        if (Input.GetKeyDown(KeyCode.LeftShift) && jibbitManager.CanDash && !isDashing)
            StartDash();
        
        
        // Check if the player is on a wall
       bool currentlyOnWall = onWall();
       if (currentlyOnWall != isOnWall)
       {
           isOnWall = currentlyOnWall;
           anim.SetBool("onWall", isOnWall); // Set animation only when the state changes
       }

       if (isOnWall)
       {
           // Wall slide settings
           if (body.velocity.y <= 0) // Ensure the player is sliding down
           {
               body.velocity = new Vector2(0, 0);
               body.gravityScale = 2;
               ResetJumpCounter();
               Debug.Log("onWall: " + isOnWall);
           }
           anim.SetBool("jumpRising", false);
       }
       else
       {
           if(isDashing)
               return;
           
           anim.SetBool("onWall", false);
           body.gravityScale = 7;
           body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
       }
       
        // Only reset jump counter once when the player first lands
        if (isGrounded())
        {
            if (!wasGrounded) // Player just landed
            {
                ResetJumpCounter(); // Reset jump counter upon landing
                wasGrounded = true; // Set grounded flag
            }

            anim.SetBool("jumpRising", false);
            anim.SetBool("jumpFalling", false);
            isInAir = false;
        }
        else
        {
            if(isDashing)
                return;
            
            wasGrounded = false; // Player is in the air

            if (body.velocity.y > 0.1f)
            {
                anim.SetBool("jumpRising", true);
                anim.SetBool("jumpFalling", false);
                isInAir = true;
            }
            else if (body.velocity.y < -0.1f)
            {
                anim.SetBool("jumpRising", false);
                anim.SetBool("jumpFalling", true);
                isInAir = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && onWall())
        {
            WallJump();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
            Jump();

        if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

        if (onWall())
        {
            if (body.velocity.y <= 0) // Only zero vertical velocity if player is sliding
            {
                body.velocity = new Vector2(0, 0);
                body.gravityScale = 2;
            }
            anim.SetBool("jumpRising", false);
        }
        else
        {
            anim.SetBool("onWall", false);
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);
        } 
    }

    private void Jump()
    {
        Debug.Log("Number jump  = " + jumpCounter);
        
        if (jumpCounter <= 0 || onWall())
            return; 
       
        jumpCounter--;
        
        SoundManager.instance.PlaySound(jumpSound);
        
        body.velocity = new Vector2(body.velocity.x, jumpPower);
        // Decrement the jump counter for the extra jump
    }
    
    public void ResetJumpCounter()
    {
        jumpCounter = jibbitManager.CanDoubleJump ? 2 : 1;
        Debug.Log("Jump counter reset to " + jumpCounter);
    }

    private void WallJump()
    {
        ResetJumpCounter();
        body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY);
        body.gravityScale = 7;

    }
    
    private void StartDash()
    {
        SoundManager.instance.PlaySound(dashSound);
        if (!isDashing)
        {
            isDashing = true;
            dashDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left; // Determine dash direction
            body.gravityScale = 7; // Keep gravity to maintain vertical motion

            // Apply a one-time dash impulse force
            body.AddForce(dashDirection * dashSpeed, ForceMode2D.Impulse);
        
            Debug.Log("Dash impulse applied. Direction: " + dashDirection + ", Force: " + dashSpeed);

            // End dash after a short duration
            Invoke(nameof(EndDash), dashDuration); // Automatically end dash after the specified duration
        }
    }
    private void EndDash()
    {
        isDashing = false;
        Debug.Log("Dash ended.");
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return !onWall();
    }
    
    // Public methods to update speed and jump power
    public void SetSpeed(float additionalSpeed)
    {
        speed = baseSpeed + additionalSpeed;
    }

    public void SetJumpPower(float additionalJumpPower)
    {
        jumpPower = baseJumpPower + additionalJumpPower;
    }
    
}
