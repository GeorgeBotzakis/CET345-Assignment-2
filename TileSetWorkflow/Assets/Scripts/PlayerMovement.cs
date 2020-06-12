using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public GameUI gameUI;
    private Vector2 rigidVel;
    public float moveSpeed = 0f;
    float horizontalMove = 0f;
    bool jump = false;
    private void Start()
    {
        rigidVel = controller.getRbVelocity();
    }
    void Update()
    {
        if (!gameUI.getPauseStatus())
        {


            rigidVel = controller.getRbVelocity();
            //print(rigidVel.y);
            horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;

            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            //HandleJumpAndFall();
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;


                /*   if(controller.getRbVelocity().y > 0f)
                  {
                      animator.SetInteger("State", 3);
                  }
                  else
                  {
                      animator.SetInteger("State", 1);
                  }*/
            }
            HandleJumpAndFall();
        }

        

        //if (!controller.getGrounded())
        //{
        //   // print("mpika");
        //    if (rigidVel.y > 0)
        //    {
        //        animator.SetInteger("State", 3); //state 3 = Jumping,  transition from idle or walking to jumping
        //    }
        //    else
        //    {
        //        animator.SetInteger("State", 1); //state 1 = Falling,  transition from jumping to falling
        //    }

        //}
    }
    void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
    void HandleJumpAndFall()
    {
        if (!controller.getGrounded())
        {
            // print("mpika");
            if (rigidVel.y > 0)
            {
                animator.SetInteger("State", 3); //state 3 = Jumping,  transition from idle or walking to jumping
            }
            else
            {
                animator.SetInteger("State", 1); //state 1 = Falling,  transition from jumping to falling
            }

        }
        //if (jump)
        //{
        //    if(controller.getRbVelocity().y > 0f)
        //    {
        //        animator.SetInteger("State", 3);
        //    }
        //    else
        //    {
        //        animator.SetInteger("State", 1);
        //    }
        //}
    }
    public void OnLanding()
    {
        animator.SetInteger("State", 0); // state 0 = idle, transition from falling to idle
       // print("aaa");
    }
    private void OnDisable()
    {
        controller.disableMovement();
    }
}
