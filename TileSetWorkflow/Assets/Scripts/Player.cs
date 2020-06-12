using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Player : MonoBehaviour      // Class for handling the player conditions, communicating with UI  (Health & Charge meter)
{
   private Animator animator;
   private CharacterController2D controller;
   // private PlayerCombat p_Combat;
    private SpriteRenderer playerSpriteRenderer;
    //private PlayerMovement p_Movement;
   public int maxHealth = 100;
   public int currentHealth;
   public Color damagedColour;
   private bool isDead = false;
   public GameUI gameUI;                     // Reference to the game's UI scipt to handle UI changes
   // string input_frame;
   bool m_playerDamaged = false;
    bool m_hasLevelKey;
    private void Awake()
    {
        playerSpriteRenderer = this.GetComponent<SpriteRenderer>();
        controller = this.gameObject.GetComponent<CharacterController2D>();
        if (this.GetComponent<Animator>() != null)
            if (this.CompareTag("Player"))
                animator = this.GetComponent<Animator>();
            else
                print(" Animator already present in object:" + this.name);
    }
    void Start()
    {
        m_hasLevelKey = false;
        currentHealth = maxHealth;
        gameUI.setMaxHealth(maxHealth);
        m_playerDamaged = false;
        //input_frame = Input.inputString;
    }

    void Update()
    {
        if (m_playerDamaged)
        {
            m_playerDamaged = false;
            // playerSpriteRenderer.color = damagedColour;
            StartCoroutine("damageColourFlash");
        }
    }

    public void takeDamage(int damage)
    {
        //if (p_Combat.getGroundAttackingState())   //if player is attacking
        //{
        //    p_Combat.cancelGroundAttackingState(false);

        //}
        //if (controller.getChargingStatus())       //if player is charging
        //{
        //    controller.setChargingStatus(false);
        //}
        //animator.SetTrigger("Hurt");
        if(currentHealth > 0 && !isDead)
        {
            m_playerDamaged = true;
            currentHealth -= damage;
            gameUI.setCurrentHealth(currentHealth);   // update Player Health bar UI     
        } 

        if (currentHealth <= 0)
        {
            onPlayerDeath();

            // end gameplay here
        }
    }
    IEnumerator damageColourFlash()
    {
        playerSpriteRenderer.color = damagedColour;
        yield return new WaitForSeconds(0.1f);
        playerSpriteRenderer.color = Color.white;
        // m_playerDamaged = false;
    }
    private void onPlayerDeath()
    {
        isDead = true;
        this.GetComponent<PlayerMovement>().enabled = false;
        this.GetComponent<PlayerCombat>().enabled = false;
        animator.SetTrigger("isDead");
        gameUI.setPausableStatus(false);   // prevent player from pausing
        gameUI.showGameOverPanel();
        //this.GetComponent<CharacterController2D>().enabled = false;
    }
    public void onPlayerWin()
    {
        this.GetComponent<PlayerMovement>().enabled = false;
        this.GetComponent<PlayerCombat>().enabled = false;
        gameUI.setPausableStatus(false);
        gameUI.showWinScreenPanel();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Pickup"))
        {
            if (collision.CompareTag("LevelKey"))
            {
                m_hasLevelKey = true;
                collision.gameObject.SetActive(false);
                gameUI.setKeyFoundText(m_hasLevelKey);
               
            }
            print("PLAYER COLLIDER IS " + this.gameObject.name);
            print("PLAYER " + collision.gameObject.name);
        }
        else
        {
            print("PIPIS , PIPA");
        }
  
    }
    public bool getKeyStatus() { return m_hasLevelKey; }
}
