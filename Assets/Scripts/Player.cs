using UnityEngine;
using UnityEngine.InputSystem;                        //rajoute les inputS. (pakage manager)

public class Player : MonoBehaviour
{


    [SerializeField] private float speed;             //float chiffre à virgule
    [SerializeField] private float xMaxSpeed;
    [SerializeField] private float yMaxSpeed;
    [SerializeField] private float jumpForce;
    

    private float direction;                          //flag qui permt de savoir si on move ou ps(v)
    private bool  canJump = false;                    //flag pr savoir si on px sauter(v)
    private bool canSwim;
    private Controls       controls;                  //classe _ (vrt-->obj)
    private Rigidbody2D    rigidbody2D;               
    private Animator       animator;
    private SpriteRenderer spriteRenderer;



    private void OnEnable()                          //Evmt au déclanchemt du player 
    {
        controls = new Controls();                   //input_act
        controls.Enable();
        controls.Main.Jump.performed   += JumpOnperformed;                                          
        controls.Main.MoveLR.performed += MoveLROnperformed;
        controls.Main.MoveLR.canceled  += MoveLROncanceled;
    }

    private void FixedUpdate()
    {
        var horizontalSpeed = Mathf.Abs(rigidbody2D.velocity.x);
        if (horizontalSpeed < xMaxSpeed)
            rigidbody2D.AddForce(new Vector2(speed * direction, 0));

        var verticalSpeed = Mathf.Abs(rigidbody2D.velocity.y);
        if (verticalSpeed < yMaxSpeed)
            rigidbody2D.AddForce(new Vector2(0, speed));

    }

    private void MoveLROncanceled(InputAction.CallbackContext obj)              //MoveOnCanceled-> player statique direction =0
    {
        direction = 0;
        if (!canSwim)   // mode run
        {
            //qd le player est à 0 on arrete l'animation running (running passe à false)
            animator.SetBool("Running", false);
        }
        // sinon mode swin, on ne change rien
    }

        private void MoveLROnperformed(InputAction.CallbackContext obj)            //MoveOnperformed-> player non statique direction !=(diffrt) 0
    {
        direction = obj.ReadValue<float>();                                    //qd <-> la direction devient non null 

        if (direction > 0)                                                     //selon la direction on Flip
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }

        if (canSwim)
        {
            animator.SetBool("Swiming", true);
        }
        else
        {
            animator.SetBool("Running", true);
        }

    }

    private void JumpOnperformed(InputAction.CallbackContext obj)                       //inputA. Jump
    {
        if (canJump)
        {
            rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);      //x = 0 , y = jumpForce _ ForcMode(type de force)
            animator.SetBool("Jumping", true);                                         //passe en mode saut
            canJump = false;                                                           //empeche de re-sauter
        }
        else if (canSwim)
        {
            rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)                                   //Qd on rentre avec le collider on px sauter
    {
        canJump = true;//px sauter


        animator.SetBool("Jumping", false);                                            //On arrête l'anim "Jumping"
        
        if (direction!=0)                                                              //selon la direct° si elle est diff de 0 on cours donc anim 
           animator.SetBool("Running", true);                                          //-> "Running"
        else                                                                           //sinn
           animator.SetBool("Running", false);                                        //-> "Running" = False
    }

    void Start()
    {
        rigidbody2D    = GetComponent<Rigidbody2D>();
        animator       = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        Debug.Log(obj.tag);
        if (obj.tag.Equals("pool"))
        {
            Debug.Log("UnderWater");
            canJump = true;
            canSwim = true;

            rigidbody2D.gravityScale = 50.0f;
            jumpForce = 200;
            speed = 100;
            yMaxSpeed = 100;

            //animator.SetBool("Running", false);
            //animator.SetBool("Jumping", false);
            //animator.SetBool("Swiming", true);
        }

    }

    private void OnTriggerExit2D(Collider2D obj)
    {
        Debug.Log(obj.tag);
        if (obj.tag.Equals("pool"))
        {
            Debug.Log("OutWater");
            canJump = false;
            canSwim = false;

            this.rigidbody2D.gravityScale = 100.0f;
            jumpForce = 400;
            speed = 470;
            yMaxSpeed = 250;


            //animator.SetBool("Running", false);
            //animator.SetBool("Jumping", false);
            //animator.SetBool("Swiming", false);
        }
    }
}