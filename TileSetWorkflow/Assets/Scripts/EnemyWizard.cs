using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EnemyWizard : MonoBehaviour
{
    public GameObject m_fireballPrefab;
    public LayerMask m_WhatIsPlayer;
    public Color onDamagedColour;
    public Transform fireballLaunchTransform;
    public Transform playerTransform;
    // private Transform entityTransform;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 m_Position;
   // private Vector2 m_fireballLaunchPos;
    private Vector2 m_PlayerPosition;
    [Range(0, 100)]
    public int m_HealthPoints;
    public float m_MovementSpeed;
    private float m_drag = 0.95f;
    public const float m_VisionRange = 10f;
    private int direction;
    private bool isDead = false;
    private bool isFlipped;
    private bool healthbarShowing = false;
    private bool isCastingFireball = false;
    [SerializeField] private bool canAttack = true;
    private float nextFireballTime = 0.0f;
    private float fireballCooldown = 3.5f;
    [SerializeField] private float m_FireballSpeed = 2f;
    private Slider wizardHPSlider;
    private CanvasGroup healthGroup;

    //private enum WIZARD_STATES
    //{
    //    IDLE = 0,
    //    PATROL = 1,
    //    ATTACKING = 2
    //}
    private void Awake()
    {

        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();
        healthGroup = this.GetComponentInChildren<CanvasGroup>();
        wizardHPSlider = this.GetComponentInChildren<Slider>();
    }
 
    void Start()
    {
       // print(wizardHPSlider.gameObject.name);
        m_HealthPoints = 100;   // health points hsould always inititalise at 100
        direction = -1;     // initital direction should point left
        nextFireballTime = Time.time;
        //m_Position = new Vector2(entityTransform.position.x, entityTransform.position.y);
       // m_fireballLaunchPos = new Vector2(fireballLaunchTransform.position.x, fireballLaunchTransform.position.y);
        wizardHPSlider.value = m_HealthPoints;
        healthGroup.alpha = 0;
        //m_PlayerPosition = new Vector2(playerTransform.position.x, m_Position.y);
    }

    void Update()
    {
       
        m_Position = new Vector2(this.transform.position.x, this.transform.position.y);
        m_PlayerPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
        //m_fireballLaunchPos = new Vector2(fireballLaunchTransform.position.x, fireballLaunchTransform.position.y);


        //  Vector2 newDestination = Vector2.MoveTowards(this.transform.position, target, m_MovementSpeed * Time.deltaTime);
        // transform.position = Vector2.MoveTowards(transform.position, target, m_MovementSpeed * Time.deltaTime);
        // transform.position += transform.right * m_MovementSpeed * direction * Time.deltaTime;
        // rb.MovePosition(Vector2.MoveTowards(rb.position, target, m_MovementSpeed * Time.fixedDeltaTime));
        lookAtPlayer();
        if (canAttack)
        {
            patrolVision();
        }
         

    }
    private void FixedUpdate()
    {
        // apply a drag on the horizontal axis to prevent entity from being pushed by the velocity of an objective
        Vector2 current_velocity = rb.velocity;
        current_velocity.x *= 1.0f - m_drag;
        rb.velocity = current_velocity;
    }
    private void patrolVision()
    {
        Color raycolor = Color.white;
        // new Vector2(m_Position.x  + m_VisionRange * direction, m_Position.y)

        RaycastHit2D raycastHit = Physics2D.Raycast(m_Position,(direction * Vector2.right).normalized, 10f, m_WhatIsPlayer);

        if(raycastHit.collider != null)
        {
            if (raycastHit.collider.gameObject.CompareTag("Player"))
            {
               
                if (nextFireballTime <= Time.time && !isCastingFireball)
                {
                    raycolor = Color.yellow;
                    isCastingFireball = true;
                    animator.SetTrigger("shootFireball");
                   // Debug.Log(raycastHit.collider.gameObject.name + " HIT BY WIZARD!");                
                    nextFireballTime = Time.time + fireballCooldown;
                }
                else
                {
                   raycolor = Color.green;
                }
               
            }
            else
            {
                raycolor = Color.blue;
                //Debug.Log(" WHIFF HIT: " + raycastHit.collider.name);
            }
   
        }
        else
        {
            raycolor = Color.red;
           
          //  print("AA");
        }
        Debug.DrawLine(m_Position, new Vector3(m_Position.x + m_VisionRange * direction, m_Position.y, 0f), raycolor);
        // new Vector2(m_Position.x + direction, m_Position.y)



    }
    public void launchFireball()
    {
        // on launch frame
        Rigidbody2D fireballInstance = Instantiate(m_fireballPrefab, fireballLaunchTransform.position, fireballLaunchTransform.rotation).GetComponent<Rigidbody2D>();
        fireballInstance.velocity = m_FireballSpeed * direction * Vector2.right;
    }
    //private void patrolMovement()
    //{
    //    Vector2 target = new Vector2(playerTransform.position.x, rb.position.y);
    //    rb.MovePosition(Vector2.MoveTowards(rb.position, target, m_MovementSpeed * Time.fixedDeltaTime));
    //}
    public void lookAtPlayer()
    {

        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;

        if (m_Position.x > m_PlayerPosition.x && isFlipped)
        {
            direction = -1;
            transform.localScale = flipped;
            transform.Rotate(0, 180f, 0f);
            healthGroup.transform.Rotate(0, 180, 0);
            isFlipped = false;
        }
        else if (m_Position.x < m_PlayerPosition.x && !isFlipped)
        {
            direction = 1;
            transform.localScale = flipped;
            transform.Rotate(0f, 180f, 0f);
            healthGroup.transform.Rotate(0, 180, 0);
            isFlipped = true;

        }
    }
    public void takeDamage(int dmg)
    {
        Debug.Log("Damage Amount: " + dmg);
        if (!isDead)
        {
            m_HealthPoints -= dmg;
            wizardHPSlider.value = m_HealthPoints;
        }
        if (m_HealthPoints > 0)
        {         
            if (healthbarShowing)
            {
                StopCoroutine("showHealthGroup");
                StartCoroutine("showHealthGroup");
            }
            else
            {
                StartCoroutine("showHealthGroup");
            }                         
            StartCoroutine("flashDamageSprite");
        }
        else
        {
            isDead = true;
            //wizardHPSlider.value = m_HealthPoints;
            StartCoroutine("showHealthGroup");
           // StopCoroutine("showHealthGroup");
           // healthGroup.alpha = 0;     
           // healthbarShowing = false;
            rb.isKinematic = true;
            this.GetComponent<Collider2D>().enabled = false;
            animator.SetTrigger("isDead");          
        }
        Debug.Log("Wizard HP: " + m_HealthPoints);
    }

    // Animation Event that runs at the last frame of the death animation, ensuring no animations are repeated
    public void onEntityDeath()
    {
        animator.ResetTrigger("isDead");
        this.gameObject.SetActive(false);
    }
    /// briefly display the canvas group of the entity's health UI
    private IEnumerator showHealthGroup()
    {
        healthbarShowing = true;
        print("TREXA TREXA");
        healthGroup.alpha = 1;
        yield return new WaitForSeconds(2f);
        healthbarShowing = false;
        healthGroup.alpha = 0;
    }
    private IEnumerator flashDamageSprite()
    {
        this.GetComponent<SpriteRenderer>().color = onDamagedColour;
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<SpriteRenderer>().color = Color.white;
    }
    /// <summary>
    /// NOT FINAL VERSION, MAY CHANGE HOW ATTACK CHECKING WORKS IN ADDITION TO ANIMATION EVENT
    /// </summary>
    public void onFireballLaunchAnimationEnd()
    {
        isCastingFireball = false;
    }
    private void OnDisable()
    {
        Destroy(this.gameObject, 2f);
    }
}
