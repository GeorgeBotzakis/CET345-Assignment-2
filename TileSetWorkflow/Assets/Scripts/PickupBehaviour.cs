using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    float m_SineSpeed = 3f;
    float m_Magnitude = 0.4f;
    private Vector3 m_startPos;
    private Vector3 m_newPosition;
    void Start()
    {
        m_startPos = this.transform.position;
       // m_newPosition = m_startPos;
    }

    // Update is called once per frame
    void Update()
    {
        //m_newPosition = transform.position;
        //m_newPosition.y += Mathf.Sin(Time.time);
        //transform.position = m_newPosition;
        transform.position = m_startPos + new Vector3(0.0f, Mathf.Sin(Time.time * m_SineSpeed) * m_Magnitude, 0.0f);
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{ 
    //    Debug.Log(collision.name);
    //}
    private void OnDisable()
    {
        Destroy(this.gameObject, 2f);
    }
}
