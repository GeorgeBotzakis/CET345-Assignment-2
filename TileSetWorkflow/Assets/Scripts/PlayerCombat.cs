using System.Linq;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private CharacterController2D controller;
    public LayerMask enemyLayers;
    public LayerMask dObjectLayers;
    public GameUI gameUI;

    public Transform attack1_Point;               // Transform of the attack point helper game object
    //const float attack1_Range = 1f;             // Range of attack 1, can be thought of as the radius of the circle collider
    const float attack1_range_x = 1.5f;           // X of the overlap box
    const float attack1_range_y = 3f;             // Y of the overlap box
    const float charge_attack1_range_x = 1.7f;
    const float charge_attack1_range_y = 1.3f;
    const int ZERO = 0;                           //constant of the zero variable
    Vector3 attack1_size = new Vector3(attack1_range_x, attack1_range_y, ZERO);
    Vector3 charge_attack_size = new Vector3(charge_attack1_range_x, charge_attack1_range_y, ZERO);
    // Combat-logic-related section
    bool isGroundAttacking = false;
    bool chargeAttackHitting = false;
    bool isAlive;
    float nextAttack1_time;
    float next_charge_attack1_time;
    float attack1_rate = 2f;
    float charge_attack1_rate = 1f;
    float charge_attack1_cooldownTime = 3f;
    float currentChargePercentage = 0;
    float currentCooldownPercentage = 50;
    float currentCooldownTime;
    public float m_minChargeAttack_power = 15f;
    public float m_maxChargeAttack_power = 75f;
    [SerializeField]
    private float m_currentChargeAttack_power;
    private float m_MaxChargeTime = 3f;                //how long attack must be charge before being released at max charge power
    private float m_ChargeRate;
    private float m_CooldownRate;
    int chargeAttackFrameCounter = 0;
    Collider2D[] firstFrameEntities = null;
    void Awake()
    {
        if (this.GetComponent<Animator>() != null)
            if(this.CompareTag("Player"))
            animator = this.GetComponent<Animator>();
        else
            print(" Animator already present in object:" + this.name);

        controller = this.gameObject.GetComponent<CharacterController2D>();
        //transform.parent.position 
    }
    private void Start()
    {
        isAlive = true;
        currentCooldownTime = 50;    // as in 50% of the cooldown UI component 
        m_currentChargeAttack_power = m_minChargeAttack_power;
        nextAttack1_time = Time.time;
        next_charge_attack1_time = Time.time;
        m_ChargeRate = (m_maxChargeAttack_power - m_minChargeAttack_power) / m_MaxChargeTime;
        m_CooldownRate = (currentCooldownTime - ZERO) / (charge_attack1_cooldownTime / charge_attack1_rate);
        currentChargePercentage = ((int)m_currentChargeAttack_power - (int)m_minChargeAttack_power) / 120;
        gameUI.setChargeCooldown(ZERO);
    }
    // Update is called once per frame
    void Update()
    {
        if (!gameUI.getPauseStatus())
        {
            if (isAlive)
            {


                currentChargePercentage = 0;
                gameUI.setCurrentCharge(currentChargePercentage);
                // when charge attack is on cooldown, set cooldown on UI
                if (next_charge_attack1_time >= Time.time)
                {
                    currentCooldownTime -= m_CooldownRate * Time.deltaTime;
                    currentCooldownPercentage = (currentCooldownTime - 0) / 100;

                    gameUI.setChargeCooldown(currentCooldownPercentage);
                }
                else
                {
                    gameUI.setChargeCooldown(ZERO);
                }
                if (m_currentChargeAttack_power >= m_maxChargeAttack_power && controller.getChargingStatus()) // max power doesn't exceed
                {
                    m_currentChargeAttack_power = m_maxChargeAttack_power;
                    currentChargePercentage = (m_currentChargeAttack_power - m_minChargeAttack_power) / 120;

                    gameUI.setCurrentCharge(currentChargePercentage);
                    Debug.Log(currentChargePercentage);
                }
                //else if(m_currentChargeAttack_power > m_maxChargeAttack_power && !controller.getChargingStatus() && !isGroundAttacking)
                //{
                //    m_currentChargeAttack_power = m_minChargeAttack_power;
                //    currentChargePercentage = (m_currentChargeAttack_power - m_minChargeAttack_power) / 120;

                //    gameUI.setCurrentCharge(currentChargePercentage);
                //    Debug.Log(currentChargePercentage);
                //}

                //All combat moves performed when character is grounded
                if (controller.getGrounded() && animator.GetInteger("State") != 1)   //make sure player is grounded and is not in the falling state
                {
                    // Check if game is paused or not



                    if (Input.GetButtonDown("Attack1") && (!isGroundAttacking && !chargeAttackHitting))
                    {
                        if (nextAttack1_time <= Time.time)
                        {
                            isGroundAttacking = true;
                            controller.setGroundAttackingStatus(isGroundAttacking);
                            animator.SetTrigger("Attack1");
                            // print("MAD CLIP");
                            nextAttack1_time = Time.time + (1 / attack1_rate);
                        }

                    }
                    //Charge Attack 1
                    else if (Input.GetButtonDown("ChargeAttack1") && !isGroundAttacking)  // when the charge button starts being held
                    {
                        if (next_charge_attack1_time <= Time.time)
                        {
                            isGroundAttacking = true;
                            controller.setChargingStatus(isGroundAttacking); //set charging state to positive, attack not landed yet
                            m_currentChargeAttack_power = m_minChargeAttack_power;
                            animator.SetInteger("State", 4);
                            print("CHARGE INIT");

                        }

                    }
                    else if (Input.GetButton("ChargeAttack1") && (isGroundAttacking && controller.getChargingStatus()))
                    {
                        // && controller.getChargingStatus()
                        // increment attack power and update UI . . .
                        m_currentChargeAttack_power += m_ChargeRate * Time.deltaTime;

                        currentChargePercentage = (m_currentChargeAttack_power - m_minChargeAttack_power) / 120;
                        gameUI.setCurrentCharge(currentChargePercentage);
                        //   if (m_currentChargeAttack_power >= m_maxChargeAttack_power)
                        //  Debug.Log(currentChargePercentage);

                        // Debug.Log(m_currentChargeAttack_power);

                        //if (Input.GetButtonDown("Jump"))  //cancel charge with jump
                        //{
                        //    //Reset charging damage power and cooldown . . .
                        //    m_currentChargeAttack_power = m_minChargeAttack_power;
                        //    controller.setChargingStatus(isGroundAttacking);
                        //    animator.SetInteger("State", 3);                 
                        //    next_charge_attack1_time = Time.time + (5 / charge_attack1_rate);   //enable charge attack cooldown
                        //}
                    }
                    else if (Input.GetButtonUp("ChargeAttack1") && isGroundAttacking)  // release button to launch attack
                    {
                        // animator.GetInteger("State") == 4
                        //controller.setChargingStatus(!isGroundAttacking);
                        //controller.setGroundAttackingStatus(!isGroundAttacking);
                        //animator.SetInteger("State", 5);    //attack state
                        if (animator.GetInteger("State") == 4)
                        {
                            if (next_charge_attack1_time <= Time.time)                // check if it 
                            {
                                currentCooldownTime = 50f;
                                gameUI.initialiseChargeCooldown();
                                print("HAI");
                                animator.SetTrigger("ChargeAttack1");
                                next_charge_attack1_time = Time.time + (charge_attack1_cooldownTime / charge_attack1_rate);
                            }
                        }
                        // isGroundAttacking = false;
                        print(isGroundAttacking);
                        // print(animator.GetInteger("State"));

                    }
                }
                //print(chargeAttackFrameCounter);
            }
        }
    }
    public void attackHit()
    {
           //print("HIT");
           //detect enemies in a circle
           // Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attack1_Point.position, attack1_Range, enemyLayers);
           Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attack1_Point.position, attack1_size, Quaternion.identity.x, enemyLayers);
           //firstFrameEnemies = hitEnemies; //store enemies hit by first frame
           //Call damage methods
           foreach (Collider2D enemy in hitEnemies)
           {
               if (enemy.gameObject != this.gameObject)
               {
                if(enemy.gameObject.layer == LayerMask.NameToLayer("Object"))
                {
                    // DESTROY OBJECT 
                    if (enemy.GetComponent<ObjectBehaviour>() != null)
                    {
                        enemy.GetComponent<ObjectBehaviour>().takeDamage(25);
                    }
                    else
                        continue;
                }
                else
                {
                    Debug.Log("XTIPISES TON " + enemy.gameObject.name);
                    //Enemy damage code here . . .
                    if (enemy.GetComponent<EnemyWizard>() != null)
                        enemy.GetComponent<EnemyWizard>().takeDamage(25);
                    else
                        continue;
                }
            }

           }          
    }
    public void charge_attack_Hit()   //Active attack frame number: 2
    {
      
       // chargeAttackHitting = true;  //helper variable for gizmo display
        if(chargeAttackFrameCounter < 2)
        {
            chargeAttackHitting = true;
            chargeAttackFrameCounter++;
        }
        else
        {
            chargeAttackFrameCounter = 0;
        }
     
        Debug.Log("method run: " + chargeAttackFrameCounter);
        if (chargeAttackFrameCounter == 1)
        {
          //  print(chargeAttackHitting);
           // Debug.Log("frame-_-1");
            //print("HIT");
            //detect enemies in a circle
            // Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attack1_Point.position, attack1_Range, enemyLayers);
            Collider2D[] hitEntities = Physics2D.OverlapBoxAll(attack1_Point.position, attack1_size, Quaternion.identity.x, enemyLayers);
            firstFrameEntities = hitEntities; //store enemies hit by first frame
            //Call damage methods
            foreach (Collider2D objectHit in hitEntities)
            {
                if (objectHit.gameObject != this.gameObject)
                {
                    Debug.Log("GAMISES TON " + objectHit.gameObject.name);
                    Debug.Log("Frame Number:" + chargeAttackFrameCounter);
                    if (objectHit.gameObject.layer == LayerMask.NameToLayer("Object"))
                    {
                        // if the entity attacked is an object, add force 
                        // based on the amount of time the player charged their attack
                        Debug.Log((int)m_currentChargeAttack_power);
                        if (controller.getPlayerFacingDirection()) //if player is facing right
                            objectHit.GetComponent<Rigidbody2D>().AddForce(transform.right * m_currentChargeAttack_power * 100);
                        else //if player is facing right
                            objectHit.GetComponent<Rigidbody2D>().AddForce(-transform.right * m_currentChargeAttack_power * 100);
                    }
                    else // if layer of enemy is hit
                    {
                        Debug.Log("PETSOKOPSES TON " + objectHit.gameObject.name);
                        Debug.Log("Frame Number: " + chargeAttackFrameCounter);
                        if (objectHit.GetComponent<EnemyWizard>() != null)
                            objectHit.GetComponent<EnemyWizard>().takeDamage((int)m_currentChargeAttack_power - 15);
                        else
                            continue;
                    }

                    //Enemy damage code here . . .


                }

            }
           // chargeAttackHitting = false;
        }
        else if (chargeAttackFrameCounter >= 2)
        {
          //  print(chargeAttackHitting);
         //   Debug.Log("frame-_-2");
            Collider2D[] hitEntities = Physics2D.OverlapBoxAll(attack1_Point.position, attack1_size, Quaternion.identity.x, enemyLayers);
            //Call damage methods
            if(hitEntities.Length != 0)
            {
                foreach (Collider2D objectHit in hitEntities)
                {
                    if (firstFrameEntities == null)
                    {
                        if (objectHit.gameObject != this.gameObject)
                        { 
                            Debug.Log("GAMISES TON " + objectHit.gameObject.name);
                            Debug.Log("Frame Number: " + chargeAttackFrameCounter);
                            // Debug.Log((int)m_currentChargeAttack_power);
                            //Enemy damage code here . . .
                            //second frame enemy excpetion code here
                            if (objectHit.gameObject.layer == LayerMask.NameToLayer("Object"))
                            {
                                Debug.Log("OBJECT FILE MOY");
                                //  Debug.Log(LayerMask.LayerToName(dObjectLayers));
                                Debug.Log((int)m_currentChargeAttack_power);
                                if (objectHit.GetComponent<ObjectBehaviour>() != null)
                                {
                                    if (controller.getPlayerFacingDirection()) //if player is facing right
                                        objectHit.GetComponent<Rigidbody2D>().AddForce(transform.right * m_currentChargeAttack_power * 100);
                                    else //if player is facing right
                                        objectHit.GetComponent<Rigidbody2D>().AddForce(-transform.right * m_currentChargeAttack_power * 100);
                                }
                                else
                                    continue;
                              
                            }
                            else  // if layer of enemy is hit
                            {
                                Debug.Log("PETSOKOPSES TON " + objectHit.gameObject.name);
                                Debug.Log("Frame Number: " + chargeAttackFrameCounter);
                                if (objectHit.GetComponent<EnemyWizard>() != null)
                                    objectHit.GetComponent<EnemyWizard>().takeDamage((int)m_currentChargeAttack_power - 15);
                                else
                                    continue;
                            }
                        }
                    }
                    else
                    {
                        if (objectHit.gameObject != this.gameObject && !firstFrameEntities.Contains<Collider2D>(objectHit)) // do not hit enemies hit by first frame again
                        {
                            Debug.Log("GAMISES TON " + objectHit.gameObject.name);
                            Debug.Log("Frame Number: " + chargeAttackFrameCounter);
                           // Debug.Log((int)m_currentChargeAttack_power);
                            //Enemy damage code here . . .
                            //second frame enemy excpetion code here
                            if (objectHit.gameObject.layer == LayerMask.NameToLayer("Object"))
                            {
                                Debug.Log("OBJECT FILE MOY");
                                //  Debug.Log(LayerMask.LayerToName(dObjectLayers));
                                Debug.Log((int)m_currentChargeAttack_power);
                                if (controller.getPlayerFacingDirection()) //if player is facing right
                                    objectHit.GetComponent<Rigidbody2D>().AddForce(transform.right * m_currentChargeAttack_power * 100);
                                else //if player is facing right
                                    objectHit.GetComponent<Rigidbody2D>().AddForce(-transform.right * m_currentChargeAttack_power * 100);
                            }
                            else
                            {
                                Debug.Log("PETSOKOPSES TON " + objectHit.gameObject.name);
                                Debug.Log("Frame Number: " + chargeAttackFrameCounter);
                                if (objectHit.GetComponent<EnemyWizard>() != null)
                                    objectHit.GetComponent<EnemyWizard>().takeDamage((int)m_currentChargeAttack_power - 15);
                                else
                                    continue;
                            }
                        }
                        //else if(enemy.gameObject != this.gameObject && firstFrameEnemies.Contains<Collider2D>(enemy)) 
                        //{
                        //    Debug.Log("WHIFF");
                        //    Debug.Log("Frame Number: " + chargeAttackFrameCounter);
                        //}
                    }


                }
            }
            // After attack hits, set the hitting state to false
            Debug.Log((int)m_currentChargeAttack_power); // ! ! !
            firstFrameEntities = null;
            chargeAttackHitting = false;
           
            //reset charge attack power, percentage and UI

            m_currentChargeAttack_power = m_minChargeAttack_power;
            currentChargePercentage = (m_currentChargeAttack_power - m_minChargeAttack_power) / 120;
            
            gameUI.setCurrentCharge(currentChargePercentage);
        }
    }
    public void onChargeAttackAnimationEnd()
    {
        //  chargeAttckHitting = false;
        Debug.Log("ANIMATION END");
       
        isGroundAttacking = false;
        //animator.ResetTrigger("ChargeAttack1");
        chargeAttackFrameCounter = 0;
        controller.setChargingStatus(isGroundAttacking);
        controller.setGroundAttackingStatus(isGroundAttacking);
        animator.SetInteger("State", 0);
        //m_currentChargeAttack_power = m_minChargeAttack_power;
        //currentChargePercentage = (m_currentChargeAttack_power - m_minChargeAttack_power) / 120;
        //gameUI.setCurrentCharge(currentChargePercentage);  



    }
    public void onAttackAnimationEnd()
    {
        isGroundAttacking = false; //finish the attacking state
        controller.setGroundAttackingStatus(isGroundAttacking);
       
        // print("TELOS");

    }
    private void OnDrawGizmosSelected()
    {
       
        if (attack1_Point == null)
            return;
        if (chargeAttackHitting)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(attack1_Point.position, attack1_Range);
        Gizmos.DrawWireCube(attack1_Point.position, new Vector3(attack1_range_x, attack1_range_y, ZERO));
        Gizmos.DrawWireCube(attack1_Point.position, new Vector3(charge_attack1_range_x, charge_attack1_range_y, ZERO));
    }
    public bool getGroundAttackingState() { return isGroundAttacking; }
    private void OnDisable()
    {
        isAlive = false;
        
    }
    //public void cancelGroundAttackingState(bool atkState)
    //{

    //    isGroundAttacking = atkState;
    //    nextAttack1_time = Time.time + (1 / attack1_rate);

    //    if ((!atkState && animator.GetInteger("State") == 4) || chargeAttackHitting)  //if state is to be canceled and is charging OR in active attack frame
    //    {
    //       // nextAttack1_time = Time.time + (1 / attack1_rate);
    //        m_currentChargeAttack_power = m_minChargeAttack_power;


    //        Debug.Log("ANIMATION CANCEL");           
    //        //animator.ResetTrigger("ChargeAttack1");
    //       // chargeAttackFrameCounter = 0;
    //        chargeAttackHitting = false;
    //        animator.ResetTrigger("ChargeAttack1");
    //        animator.ResetTrigger("Attack1");
    //        animator.SetInteger("State", 0); //set to idle state if charge is cancelled 
    //        controller.setChargingStatus(isGroundAttacking);
    //        controller.setChargingStatus(isGroundAttacking);
    //        // onChargeAttackAnimationEnd();                        
    //        // animator.SetInteger("State",  0);  
    //    }
    //}
}
