using UnityEngine;                                                                                                                                              
using UnityEngine.InputSystem;                        //rajoute les inputS. (pakage manager)                                                                              
public class Player : MonoBehaviour                                                 
{

                                                                                                                                                        /* declaration des attributs de ma class player accessible a unity [SerializeField] */
    [SerializeField] private float speed;             //                                                                                                
    [SerializeField] private float xMaxSpeed;         // limitation de la vitesse en courant                                                                          
    [SerializeField] private float yMaxSpeed;         // limitation de la vitesse vertical pour limiter la hauteur de saut                              
    [SerializeField] private float jumpForce;         // force donnée au saut                                                                            

    private float jumpForceSave;
    private float speedSave;
    private float yMaxSpeedSave;

    private float direction;                          // flag qui permet de savoir si on se deplace (!=0) et dans quel sens <0 a gauche >0 adroite      /* declaration des variables private de ma class player */ 
    private bool  canJump = false;                    // flag pr savoir si on px sauter(v)                                                            
    private bool  canSwim = false;                    // flag pr savoir si on px nager(v)      

    // composants unity
    private Controls       controls;                  // gestionnaire de actions utilisées pour gerer les touches                                       /* declaration des composants unity gerant le comportement du player */          
    private Rigidbody2D    rigidbody2D;               // gestionnaire comportement physique du player         
    private Animator       animator;                  // gestion de transition des etats de l'animation    
    private SpriteRenderer spriteRenderer;            // gestion des animations                                                                                           


    void Start()
    {                                                 // initialisation des composants
        rigidbody2D    = GetComponent<Rigidbody2D>();
        animator       = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void OnEnable()                           // Event au déclenchement du player 
    {
        controls = new Controls();                                                                                   /* initialisation input action ; abonnement aux evenements generer par le clavier */
        controls.Enable();
        controls.Main.Jump.performed   += JumpOnperformed;                                                           /* associations (binding) */                                 
        controls.Main.MoveLR.performed += MoveLROnperformed;
        controls.Main.MoveLR.canceled  += MoveLROncanceled;
    }

    private void FixedUpdate()                                                                                       /* event declenché a chaque changement de frame du sprite, on va s'en servir pour gerer la limite de vitesse  */                                   
    {

        var horizontalSpeed = Mathf.Abs(rigidbody2D.velocity.x);                                                     /* recup de la vitesse du player : rigidbody2D.velocity.x (>0 a droite <0 a gauche)          */                                                                                                                                                                                                               
        if (horizontalSpeed < xMaxSpeed)                                                                             /* on check que abs( rigidbody2D.velocity.x ) est inferieur a vitesse limite que l'on impose */                                                    
            rigidbody2D.AddForce(new Vector2(speed * direction, 0));                                                 /* si vitesse inferieur a limite                                                             */
                                                                                                                     /*     on ajoute a la vitesse                                                                */
                                                                                                                     /* sinon on fait rien                                                                        */
                                                                                                                                                                                         
        var verticalSpeed = Mathf.Abs(rigidbody2D.velocity.y);                                                       /* On fait la meme chose pour la vitesse verticale afin de limiter le saut                   */                                                    
        if (verticalSpeed < yMaxSpeed)                                                  
            rigidbody2D.AddForce(new Vector2(0, speed));                          

    }


    private void OnCollisionEnter2D(Collision2D col)    // Evenement déclancher à la collision
    {
        canJump = true;                                                                                             /* on peux de nouveau sauter quand on touche un collider     */


        animator.SetBool("Jumping", false);                                                                         /* On arrête l'anim "Jumping"                                */

        if (direction != 0)                                                                                         /* selon la direct° si elle est diff de 0 on cours donc anim */
            animator.SetBool("Running", true);                                                                      /* -> mode "Running"= true                                   */
        else                                                                                                        /* sinon                                                     */
            animator.SetBool("Running", false);                                                                     /* -> mode standing ,"Running" = False                       */
    }

    private void MoveLROnperformed(InputAction.CallbackContext obj)   // MoveOnperformed-> player non statique direction !=(different) 0
    {
        direction = obj.ReadValue<float>();                                                                         /* qd la touche < ou > est appuyée la direction devient non null */

        if (direction > 0)                                                                                          /* si la  direction est plus grande que 0, le sprite en forme normal (pas FLip) */
        {
            spriteRenderer.flipX = false;
        }
        else                                                                                                        /* sinon on l'inverse (le sprite est Flippé) */
        {
            spriteRenderer.flipX = true;
        }
    }


    private void MoveLROncanceled(InputAction.CallbackContext obj)              // MoveOnCanceled->(touche de deplacement relaché, player devient static : direction =0
    {
        direction = 0;
        if (!canSwim)                                                           // "!" = not                         /* si il ne nage pas, il cours    */
        {                                                    
            animator.SetBool("Running", false);                                                                      /* qd le player est à 0 on arrete l'animation running (running passe à false) */
        }
    }


    private void JumpOnperformed(InputAction.CallbackContext obj)                      // inputAction. Jump
    {
        if (canJump)                                                                                                                                  /* si j'ai le droit de sauter           */
        {                                                                                                 
            rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);      // x = 0 , y = jumpForce _ ForcMode(type de force)             /* j'ajoute l'impulsion de saut         */
            animator.SetBool("Jumping", true);                                         // passe en mode saut                                          /* je declenche l'anim jump             */
            canJump = false;                                                           // empeche de re-sauter                                        /* je notifie que je suis en mode saut  */
        }
        else if (canSwim)                                                              // peux multiple jump seulement si on est dans l'eau
        {
            rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);                                                                     
        }
    }


    private void OnTriggerEnter2D(Collider2D obj)                                      // On a défini une zone ( Plouf_Square ) avec un trigger activée par l'entrée du player   
    {                                                                                  // ( Plouf_Square )est defini avec un tag="pool"

        if (obj.tag.Equals("pool"))                                                                                                                    /* Si son tag est "pool" on est sur le bon collider on va nager */
        {
            canJump = true;
            canSwim = true;
            
            // on sauve les constantes physiques hors de l'eau                                                                                         /* je sauve mes valeurs pour pouvoir les reutiliser lors de a transition inverse */
            jumpForceSave = jumpForce;
            speedSave     = speed;
            yMaxSpeedSave = yMaxSpeed;

            // on modifie le comportement physique du rigidbody pour simuler la nage
            rigidbody2D.gravityScale = 50.0f;
            jumpForce = 200;
            speed = 100; 
            yMaxSpeed = 100;
        }

    }

    private void OnTriggerExit2D(Collider2D obj)                                // on sort de la zone trigger ( Plouf_Square )
    { 
       
        if (obj.tag.Equals("pool"))                                                                                                                    /* on check le tag pour etre sur de sortir de l'eau */
        {
           
            canJump = false;
            canSwim = false;

                                                                                                                                                        /* on rebascule sur les valeurs physiques hors de l'eau */
            rigidbody2D.gravityScale = 100.0f;
            jumpForce = jumpForceSave;
            speed = speedSave;
            yMaxSpeed = yMaxSpeedSave;

        }
    }
}