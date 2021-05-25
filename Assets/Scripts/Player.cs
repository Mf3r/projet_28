using UnityEngine;                                                                                                                                              
using UnityEngine.InputSystem;                        //rajoute les inputS. (pakage manager)                                                                              
public class Player : MonoBehaviour                                                 
{

    /* declaration des attributs de ma class player accessible a unity [SerializeField] */                                                                                                                                                    
    [SerializeField] private float speed;                                                                                                          
    [SerializeField] private float xMaxSpeed;                                                         // limitation de la vitesse en courant                                                                          
    [SerializeField] private float yMaxSpeed;                                                         // limitation de la vitesse vertical pour limiter la hauteur de saut                              
    [SerializeField] private float jumpForce;                                                         // force donnée au saut                                                                            

    private float jumpForceSave;
    private float speedSave;
    private float yMaxSpeedSave;

    /* declaration des variables private de ma class player */
    private float direction;                                                                           // flag qui permet de savoir si on se deplace (!=0) et dans quel sens <0 a gauche >0 adroite      
    private bool  canJump = false;                                                                     // flag pr savoir si on px sauter(v)                                                            
    private bool  canSwim = false;                                                                     // flag pr savoir si on px nager(v)      

    /* declaration des composants unity gerant le comportement du player */                            // composants unity
    private Controls       controls;                                                                   // gestionnaire de actions utilisées pour gerer les touches                                                 
    private Rigidbody2D    rigidbody2D;                                                                // gestionnaire comportement physique du player         
    private Animator       animator;                                                                   // gestion de transition des etats de l'animation    
    private SpriteRenderer spriteRenderer;                                                             // gestion des animations                                                                                           


    void Start()
    {                                                 // initialisation des composants
        rigidbody2D    = GetComponent<Rigidbody2D>();
        animator       = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void OnEnable()                                                                            // Event au déclenchement du player 
    {
        /* initialisation input action ; abonnement aux evenements generer par le clavier */
        /* associations (binding)                                                         */

        controls = new Controls();                                                                                  
        controls.Enable();
        controls.Main.Jump.performed   += JumpOnperformed;                                                                                           
        controls.Main.MoveLR.performed += MoveLROnperformed;
        controls.Main.MoveLR.canceled  += MoveLROncanceled;
    }

    private void FixedUpdate()                                                                                                                       
    {
        /* event declenché a chaque changement de frame du sprite, on va s'en servir pour gerer la limite de vitesse  */
        /* recup de la vitesse du player : rigidbody2D.velocity.x (>0 a droite <0 a gauche)                           */
        /* on check que abs( rigidbody2D.velocity.x ) est inferieur a vitesse limite que l'on impose                  */
        /* si vitesse inferieur a limite                                                                              */
        /*     on ajoute a la vitesse                                                                                 */
        /* sinon on fait rien                                                                                         */
        /* On fait la meme chose pour la vitesse verticale afin de limiter le saut                                    */

        var horizontalSpeed = Mathf.Abs(rigidbody2D.velocity.x);                                                                                                                                                                                                                                                         
        if (horizontalSpeed < xMaxSpeed)                                                                                                                 
            rigidbody2D.AddForce(new Vector2(speed * direction, 0));                                            
                                                                                                     
                                                                                                              
                                                                                                                                                                                         
        var verticalSpeed = Mathf.Abs(rigidbody2D.velocity.y);                                                                                                         
        if (verticalSpeed < yMaxSpeed)                                                  
            rigidbody2D.AddForce(new Vector2(0, speed));                          

    }


    private void OnCollisionEnter2D(Collision2D col)                                             // Evenement déclancher à la collision
    {
        /* on peux de nouveau sauter quand on touche un collider     */
        /* On arrête l'anim "Jumping"                                */
        /* selon la direct° si elle est diff de 0 on cours donc anim */
        /* -> mode "Running"= true                                   */
        /* sinon                                                     */
        /* -> mode standing ,"Running" = False                       */

        canJump = true;                                                                                          


        animator.SetBool("Jumping", false);                                                                    

        if (direction != 0)                                                                               
            animator.SetBool("Running", true);                                                   
        else                                                                                                       
            animator.SetBool("Running", false);                                                                    
    }

    private void MoveLROnperformed(InputAction.CallbackContext obj)                            // MoveOnperformed-> player non statique direction !=(different) 0
    {
        /* qd la touche < ou > est appuyée la direction devient non null                */
        /* si la  direction est plus grande que 0, le sprite en forme normal (pas FLip) */
        /* sinon on l'inverse (le sprite est Flippé)                                    */

        direction = obj.ReadValue<float>();
        animator.SetBool("Running", true);                                                      //Au changement de direction l'évènement MoveLROnc. s'intercalle, je dois remettre l'animation en route

        if (direction > 0)                                                                                          
        {
            spriteRenderer.flipX = false;
        }
        else                                                                                                        
        {
            spriteRenderer.flipX = true;
        }
    }


    private void MoveLROncanceled(InputAction.CallbackContext obj)                              // MoveOnCanceled->(touche de deplacement relaché, player devient static : direction =0
    {
        /* si il ne nage pas, il cours                                                */
        /* qd le player est à 0 on arrete l'animation running (running passe à false) */

        direction = 0;
        if (!canSwim)                                                                           // "!" = not                        
        {                                                    
            animator.SetBool("Running", false);
        }
    }


    private void JumpOnperformed(InputAction.CallbackContext obj)                               // inputAction. Jump
    {
        /* si j'ai le droit de sauter                                                   */
        /*    j'ajoute l'impulsion de saut                                              */
        /*    je declenche l'anim jump                                                  */
        /*    je notifie que je suis en mode saut et que j'ai pas le droit de re-sauter */
        /* sinon si je suis en mode swim                                                */
        /*    je peux toujour donner une impulsion vers le haut                         */
        /*    j'ajoute l'impulsion de saut                                              */
 
        if (canJump)                                                                                                                               
        {
            rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);               // x = 0 , y = jumpForce _ ForcMode(type de force)            
            animator.SetBool("Jumping", true);                                                  // passe en mode saut                                          
            canJump = false;                                                                    // empeche de re-sauter                                        
        }
        else if (canSwim)                                                                       // peux multiple jump seulement si on est dans l'eau
        {
            rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);                                                                     
        }
    }


    private void OnTriggerEnter2D(Collider2D obj)                                               // On a défini une zone ( Plouf_Square ) avec un trigger activée par l'entrée du player   
    {                                                                                           // ( Plouf_Square )est defini avec un tag="pool"
        /* Si son tag est "pool" on est sur le bon collider on va sauter et nager            */
        /*   je sauve mes valeurs pour pouvoir les reutiliser lors de a transitions inverses */

        if (obj.tag.Equals("pool"))                                                                                                                    
        {
            canJump = true;
            canSwim = true;
            
                                                                                                 // on sauve les constantes physiques hors de l'eau                                                                                         
            jumpForceSave = jumpForce;
            speedSave     = speed;
            yMaxSpeedSave = yMaxSpeed;

                                                                                                 // on modifie le comportement physique du rigidbody pour simuler la nage
            rigidbody2D.gravityScale = 30.0f;
            jumpForce = 150;
            speed = 40; 
            yMaxSpeed = 150;
        }

    }

    private void OnTriggerExit2D(Collider2D obj)                                                 // on sort de la zone trigger ( Plouf_Square )
    {
        /* on check le tag pour etre sur de sortir de l'eau */
        /* on rebascule sur les valeurs physiques hors de l'eau */

        if (obj.tag.Equals("pool"))                                                                                                                    
        {
           
            canJump = false;
            canSwim = false;

                                                                                                                                                       
            rigidbody2D.gravityScale = 100.0f;
            jumpForce = jumpForceSave;
            speed = speedSave;
            yMaxSpeed = yMaxSpeedSave;

        }
    }
}