using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    //[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private LayerMask m_WhatIsWall;                             // A mask determining what is a wall to the character
    //[SerializeField] private LayerMask m_WhatIsWall;                           
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    //[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
   // [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    const float k_GroundedRadius = .21f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    private bool m_Charging = false;                        //charging attack state of player
    private bool m_Ground_attacking = false;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    private void Awake()
    {
     
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;
        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {

            if (colliders[i].gameObject != gameObject)
            {
    
                m_Grounded = true;
                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                   // print(colliders[i].gameObject.name);
                }
                   
            }
        }
    }


    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
 /*       if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }*/

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
          
            if (getChargingStatus() || getGroundAttackingStatus())
            {
                m_Rigidbody2D.velocity = Vector3.zero;
            }
            else
            {
                // And then smoothing it out and applying it to the character
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
            }
        
           // print(m_Rigidbody2D.velocity);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            if (!getChargingStatus() && !getGroundAttackingStatus())
            {
                print("JUMP");
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
   
        }
    }
    private void OnDrawGizmosSelected()
    {
        if(getGrounded())
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundedRadius);
       // Gizmos.DrawRay()
    }
  
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public Vector2 getRbVelocity()
    {
        return m_Rigidbody2D.velocity;
    }

    public bool getGrounded()
    {
        return m_Grounded;
    }
    public void disableMovement()
    {
        m_Rigidbody2D.velocity = Vector2.zero;
        m_Rigidbody2D.isKinematic = true;
        this.GetComponent<BoxCollider2D>().enabled = false;
      
    }
    public bool getChargingStatus() { return m_Charging; }
    public void setChargingStatus(bool status) {  m_Charging = status; }
    public void setGroundAttackingStatus(bool status) { m_Ground_attacking = status; }
    public bool getGroundAttackingStatus() { return m_Ground_attacking; }
    public bool getPlayerFacingDirection() { return m_FacingRight; }
    // If crouching
    /* if (crouch)
     {
         if (!m_wasCrouching)
         {
             m_wasCrouching = true;
             OnCrouchEvent.Invoke(true);
         }

         // Reduce the speed by the crouchSpeed multiplier
         move *= m_CrouchSpeed;

         // Disable one of the colliders when crouching
         if (m_CrouchDisableCollider != null)
             m_CrouchDisableCollider.enabled = false;
     }
     else
     {
         // Enable the collider when not crouching
         if (m_CrouchDisableCollider != null)
             m_CrouchDisableCollider.enabled = true;

         if (m_wasCrouching)
         {
             m_wasCrouching = false;
             OnCrouchEvent.Invoke(false);
         }
     }*/

    // Move the character by finding the target velocity

    // print(move);
    // Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
}