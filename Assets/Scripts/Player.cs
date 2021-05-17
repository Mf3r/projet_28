using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    /*
    [SerializeField] private int myInt;

    [SerializeField] private string myString;

    private Vector3 position;
    */

    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpForce;

    private float direction;

    private Controls controls;

    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool canJump = false;

    private void OnEnable()
    {
        controls = new Controls();
        controls.Enable();
        controls.Main.Jump.performed += JumpOnperformed;
        controls.Main.MoveLR.performed += MoveLROnperformed;
        controls.Main.MoveLR.canceled += MoveLROncanceled;
    }

    /// <summary>
    /// Ceci est ma fonction est ma fonction quand je relache le bouton de mouvement
    /// </summary>
    /// <param name="obj"></param>
    private void MoveLROncanceled(InputAction.CallbackContext obj)
    {
        direction = 0;
    }

    private void MoveLROnperformed(InputAction.CallbackContext obj)
    {
        direction = obj.ReadValue<float>();
        if (direction > 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
        // OU
        //spriteRenderer.flipX = direction < 0;
    }

    private void JumpOnperformed(InputAction.CallbackContext obj)
    {
        //Debug.Log("Je saute !");
        if (canJump)
        {
            rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            canJump = false;
        }

        //transform.position += new Vector3(0,1,0);
    }

    // Start is called before the first frame update
    void Start()
    {
        // rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.flipX = true;
        //animator.SetFloat("Speed", 1.0f);
        /*
        Debug.Log(myInt);
        Debug.Log(myString);
        if (myInt > 5)
        {
            Debug.Log("Ça fait plus de 5");
        }

        position = new Vector3(0, 0, 0);

        var myVector = new Vector3(1, 1, 1);
        var multiply = myVector * 3;
        Debug.Log(multiply);
        //transform.position = myVector;
        transform.position = transform.position + new Vector3(1, 0, 0);
        // Ce que j’écris ici se lancera lorsque le jeu sera lancé
        Sum(8,7);
        */
    }

    // Update is called once per frame
    void Update()
    {
        //rigidbody2D.velocity = new Vector2(100, 100);
        /*
        var mySecondInt = 0;
        Debug.Log(mySecondInt++);
        */
        // S’exécute toutes les frames
        /*
        position.x++;
        transform.position = position;
        Debug.Log(position);
        if (transform.position.x > 0)
        {
            Debug.Log("Vrai");
        }
        else if (transform.position.x == 0)
        {
            Debug.Log("Égalité");
        }
        // else if (transform.position.x < 0)
        
        else
        {
            Debug.Log("Faux");
        }
        //Debug.Log(transform.position.x);
        */
        var hit = Physics2D.Raycast(transform.position, new Vector2(0, -1), 0.001f);
        //Debug.DrawRay(transform.position, new Vector2(0, -1) * 0.001f);
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

    void OnApplicationQuit()
    {
    }

    private void Sum(int numberLeft, int numberRight)
    {
        var result = numberLeft + numberRight;
        //Debug.Log(result);
    }
}