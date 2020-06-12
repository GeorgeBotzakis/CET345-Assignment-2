using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectBehaviour : MonoBehaviour
{
    public Color m_DamagedFlashColour;
    public CanvasGroup m_ObjectCanvasGroup;
    [SerializeField] private int m_HealthPoints;
    private Slider m_HealthBarSlider;
    private bool m_IsDestroyed = false;
    [SerializeField] private bool isUnbreakable = false;
    private int m_DamageMultiplier;              // damage multiplier depends on the mass of the rigidbody of the object
    private bool m_healthGroupShowing = false;
    private void Awake()
    {
        m_HealthBarSlider = m_ObjectCanvasGroup.GetComponentInChildren<Slider>();
    }
    void Start()
    {
        m_ObjectCanvasGroup.alpha = 0;
        if (this.gameObject.CompareTag("SmallBox"))
        {
            m_HealthPoints = 50;
        }
        else
        {
            m_HealthPoints = 100;
        }
        m_HealthBarSlider.maxValue = m_HealthPoints;
        // Get the object's rigidbody mass and use it differentiate the damage multiplier
        m_DamageMultiplier = (int)this.GetComponent<Rigidbody2D>().mass;

        if( m_DamageMultiplier == 10)
        {
            m_DamageMultiplier = 5;
        }
        else if( m_DamageMultiplier == 5)
        {
            m_DamageMultiplier = 3;
        }
        if (isUnbreakable && this.CompareTag("UnbreakableBox"))
        {
            m_ObjectCanvasGroup.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takeDamage(int dmgAmount)
    {
        if (!isUnbreakable)
        {
            Debug.Log(" BOX Damage received: " + dmgAmount);
            if (!m_IsDestroyed)
            {
                m_HealthPoints -= dmgAmount;
                m_HealthBarSlider.value = m_HealthPoints;
                if (m_HealthPoints > 0)
                {
                    if (!m_healthGroupShowing)
                    {
                        StartCoroutine("showHealthCanvas");
                    }
                    else
                    {   // restart coroutine
                        StopCoroutine("showHealthCanvas");
                        StartCoroutine("showHealthCanvas");
                    }
                    StartCoroutine("flashDamageSprite");
                }
                else
                {
                    m_IsDestroyed = true;
                    this.gameObject.SetActive(false);
                }

            }
        }
    }
    private IEnumerator flashDamageSprite()
    {
        this.GetComponent<SpriteRenderer>().color = m_DamagedFlashColour;
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<SpriteRenderer>().color = Color.white;
    }
    private IEnumerator showHealthCanvas()
    {
        m_healthGroupShowing = true;
        m_ObjectCanvasGroup.alpha = 1;
        yield return new WaitForSeconds(5f);
        m_ObjectCanvasGroup.alpha = 0;
        m_healthGroupShowing = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (!isUnbreakable)
        {

           if (collision != null)
           {
             if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
             {
               Vector2 m_rb_vel = this.GetComponent<Rigidbody2D>().velocity;
               Debug.Log(m_rb_vel);
               if (collision.gameObject.CompareTag("Wizard"))
               {
                 print("OPA ENAS MAGOS!!!! " + collision.gameObject.name);
                 if (!collision)
                   return;
                 else
                 {
                            if (Mathf.Abs(m_rb_vel.x) >= 1.5f)
                            {

                                EnemyWizard wizardHit = collision.GetComponent<EnemyWizard>();
                                if (!wizardHit)
                                    return;
                                int roundedObjectVelocity = Mathf.CeilToInt(Mathf.Abs(m_rb_vel.x));
                                Debug.Log("HORIZONTAL DMG: " + roundedObjectVelocity);
                                if (Mathf.Abs(m_rb_vel.y) >= 1f)
                                {
                                    int roundedObjectFallVelocity = Mathf.CeilToInt(Mathf.Abs(m_rb_vel.y));
                                    Debug.Log("FALL DMG: " + roundedObjectFallVelocity);
                                    if (this.CompareTag("SmallBox"))
                                        wizardHit.takeDamage(roundedObjectFallVelocity * m_DamageMultiplier * 4);
                                    else
                                        wizardHit.takeDamage(roundedObjectFallVelocity * m_DamageMultiplier * 2);
                                    //return;
                                }
                                if (roundedObjectVelocity >= 8f)
                                {
                                    // temp damage value
                                    if (this.CompareTag("SmallBox"))
                                        wizardHit.takeDamage(roundedObjectVelocity * m_DamageMultiplier);
                                    else
                                        wizardHit.takeDamage(roundedObjectVelocity * m_DamageMultiplier);

                                    if (this.CompareTag("SmallBox"))
                                        this.takeDamage((int)Mathf.Pow(5, m_DamageMultiplier));
                                }
                                else if (roundedObjectVelocity < 8f && roundedObjectVelocity >= 5)
                                {
                                    // if object velocity on impact is between 2 and 4
                                    // temp damage value
                                    if (this.CompareTag("SmallBox"))
                                        wizardHit.takeDamage(roundedObjectVelocity * m_DamageMultiplier);
                                    else
                                        wizardHit.takeDamage(roundedObjectVelocity * m_DamageMultiplier);
                                    if (this.CompareTag("SmallBox"))
                                    {
                                        this.takeDamage((int)Mathf.Pow(5, m_DamageMultiplier));
                                    }

                                }
                                else
                                {
                                    if (roundedObjectVelocity >= 1.5f && roundedObjectVelocity < 5)
                                    {
                                        wizardHit.takeDamage(20);
                                        if (this.CompareTag("SmallBox"))
                                        {
                                            this.takeDamage(10);
                                        }
                                    }
                                    else
                                    {
                                        Debug.Log("0% dmg");
                                    }
                                    //else if(roundedObjectVelocity < 1.5f && roundedObjectVelocity >=1)
                                    //{
                                    //    wizardHit.takeDamage(10);
                                    //}

                                    return;
                                    // if velocity is less and equal than 1
                                    // wizardHit.takeDamage(roundedObjectVelocity * 1);
                                }

                            }
                            else
                                return;      
                 }
               }
               else
               {
                 return;
               }
             }
           }
        }
       
    }
    private void OnDisable()
    {
        Destroy(this.gameObject, 2f);
    }
}
