using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
   
    void Start()
    {
        Destroy(this.gameObject, 10f);   // destroy a fireball after 10 seconds of lifetime
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            // on collision with the player hitbox / collider (NON-TRIGGER)
            if (collision.gameObject.CompareTag("Player")) 
            {
                // acess player script to reduce health
                Debug.Log("player damaged");
                collision.GetComponent<Player>().takeDamage(15);
                this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Object"))
            {
                //ObjectBehaviour 
                collision.GetComponent<ObjectBehaviour>().takeDamage(25);
                //  collision.gameObject.SetActive(false);
                this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                // Collision with ground or wall
                this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
        }
    }
}
