using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{


    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask ground; // Permet de sélectionner un ou plusieurs layers pour notre sol

    private float direction;

    private Controls controls;
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    //private bool isOnGround = false;

    private bool canJump = false;

    private void OnEnable()
    {
        controls = new Controls();
        controls.Enable();
        controls.Main.Jump.performed += JumpOnperformed;
        controls.Main.MoveLR.performed += MoveLROnperformed;
        controls.Main.MoveLR.canceled += MoveLROncanceled;
    }


    private void MoveLROncanceled(InputAction.CallbackContext obj)
    {
        direction = 0;
        animator.SetBool("Runing", false);

    }

    private void MoveLROnperformed(InputAction.CallbackContext obj)
    {
        direction = obj.ReadValue<float>();
        if (direction > 0)
        {
            spriteRenderer.flipX = false;
            animator.SetBool("Runing", true);

        }
        else
        {
            spriteRenderer.flipX = true;
            animator.SetBool("Runing", true);

        }

    }

    private void JumpOnperformed(InputAction.CallbackContext obj)
    {

        if (canJump)
        {
            rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            canJump = false;
            animator.SetBool("Jumping", true);
        }


    }

    // Start is called before the first frame update
    void Start()
    {

        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }


    void Update()
    {


        var hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), 0.01f);

        if (hit.collider != null)
        {
            canJump = true;
        }
        else
        {
            canJump = false;

        }
    }

    private void FixedUpdate()
    {
        var horizontalSpeed = Mathf.Abs(rigidbody2D.velocity.x);
        if (horizontalSpeed < maxSpeed)
            rigidbody2D.AddForce(new Vector2(speed * direction, 0));

    }

    /*private void OnCollisionEnter2D(Collision2D other)
    {
        // Booléen vérifiant si le layer sur lequel on a atteri appartient bien au layerMask "ground"
        var touchGround = ground == (ground | (1 << other.gameObject.layer));
        // Booléen vérifiant que l'on collisionne avec une surface horizontale
        var touchFromAbove = other.contacts[0].normal == Vector2.up;
        if (touchGround && touchFromAbove)
        //if (other.gameObject.CompareTag("Ground") == true)
        {
            isOnGround = true;
        }

    }

    /// <summary>
    /// Exécutée lorsque le bouton de saut est appuyé
    /// </summary>
    /// <param name="obj"></param>
    private void JumpOnperf(InputAction.CallbackContext obj)
    {
        // Si isOnGround est vrai
        if (isOnGround)
        {
            // On saute
            rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            // Et on désactive la possibilité de sauter à nouveau
            isOnGround = false;
        }
    }*/

}