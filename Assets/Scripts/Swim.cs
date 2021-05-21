using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swim : MonoBehaviour
{


    [SerializeField] private float speed;             //float chiffre à virgule
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;


    private float direction;                          //flag qui permt de savoir si on move ou ps(v)
    private bool canJump = false;                    //flag pr savoir si on px sauter(v)


    private Controls controls;                  //classe _ (vrt-->obj)
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;


    public void OnTriggerEnter2D(Collider2D collision)           //Detection de la zone
    {
        Debug.Log("Under Water ");
        canJump = true;
    }

    
    

        //useGraviy = false ?
        //if Ploof_Square is true
        //rigidbody
    


}