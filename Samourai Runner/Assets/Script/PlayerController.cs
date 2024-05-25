using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerController : MonoBehaviour
{
    private float horizontal;
    public float speed = 8f;
    public float jumpingPower = 16f;
    private bool isFacingRight = true;
    private Animator anim;
    public float radius;
    public LayerMask Ennemie;
    public GameObject Ennemies;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    public Collider2D collider;

    public Collider2D enemyCollider;
    
        
    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 8f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    private bool isWallSliding;
    private float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(3f,9f);


    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private float JumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private bool doubleJump;
    
    public GameObject[] enemies;

    public Text timerText;
    public Text medalText;
    private float levelTime;
    private bool levelCompleted;

   
    public float goldTime = 7.0f;
    public float silverTime = 12.0f;

    public Image I_Bronze;

    public Image I_Argent;

    public Image I_Or;
    public Canvas canvasNiveauFini;
    public GameObject NiveauSuivant;
    public Image Cadenas;
    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Ennemies");
         levelTime = 0f;
        levelCompleted = false;
        canvasNiveauFini.enabled = false;
       

    }
    void Update()
    {

        if (isDashing)
        {
            return;
        }


            anim = GetComponent<Animator>();
            horizontal = Input.GetAxisRaw("Horizontal"); 
            WallSlide();

        WallJump();


            

        if (IsGrounded() && !Input.GetButton("Jump"))
        {
            doubleJump = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (IsGrounded() || doubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

                doubleJump = !doubleJump;
            }
        }

        if (IsGrounded())
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = JumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f )
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            jumpBufferCounter = 0f;

        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimeCounter = 0f;
        }

        if (!levelCompleted)
        {
            levelTime += Time.deltaTime;
            UpdateTimerDisplay();
        }


        OnEnemyKilled();

       
       if(!isWallJumping)
       {
        Flip();
        }
        if (horizontal!= 0)
        {
        anim.SetBool("IsRunning",true);
        }
        else
        {
            anim.SetBool("IsRunning",false);
        }

        if(!IsGrounded())
        {
            anim.SetBool("isJumping",true);
        }
        else
        {
            anim.SetBool("isJumping",false);
        }
        
        if (Input.GetButtonDown("Attack"))
        {

            anim.SetBool("IsAttack",true);  

        }

        if (Input.GetButtonDown("Attack") && enemyCollider != null){

            enemyCollider.GetComponent<Animator>().Play("Death");
            enemyCollider = null;
            
        }
        
        if(Input.GetKeyDown(KeyCode.K) && canDash)
        {
            StartCoroutine(Dash());
        }
    }


    void UpdateTimerDisplay()
    {
  
        int minutes = Mathf.FloorToInt(levelTime / 60F);
        int seconds = Mathf.FloorToInt(levelTime % 60F);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void CompleteLevel()
    {
        levelCompleted = true;
        AssignMedal();
    }

    void AssignMedal()
    {
        if (levelTime < goldTime)
        {
            medalText.text = "Medaille D'Or";
           
        }
        else if (levelTime < silverTime)
        {
            medalText.text = "Medaille D'Argent";
           
        }
        else
        {
            medalText.text = "Medaille De Bronze";
           
        }
    }

    public void OnEnemyKilled()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                return;
            }
        }

        PlayerWins();
        CompleteLevel();
    }

    void PlayerWins()
    {
        

        if (levelTime < goldTime)
        {
           I_Or.enabled = true;
           
        }
        else if (levelTime < silverTime)
        {
            I_Argent.enabled = true;
            NiveauSuivant.SetActive(true);
        }
        else
        {
            I_Bronze.enabled = true;
            Cadenas.enabled = true;
        }

        canvasNiveauFini.enabled = true;

        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

    }


    private void FixedUpdate()
    {

        if (isDashing)
        {
            return;
        }
        
        if(!isWallJumping)
       {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        

    }

    private bool IsGrounded()
    {

        return Physics2D.OverlapCircle(groundCheck.position,0.2f,groundLayer);

    }

    private void Flip()
    {
        if(isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

    }
    
    private void OnTriggerEnter2D(Collider2D collider)
    {
        
        if (((1 << collider.gameObject.layer) & Ennemie) != 0 )
        {

            enemyCollider = collider;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
     
        if (collider == enemyCollider)
        {
           
            enemyCollider = null;
        }
    }

   

    public void endattack(){

        anim.SetBool("IsAttack",false);


    }

    private IEnumerator Dash()
    {

        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
   
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f,wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && IsGrounded() && horizontal != 0f)
        {

            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));

        }
        else
        {
            isWallSliding = false;
        }


    }


    private void WallJump()
    {

        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;
            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -=Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

    
            Invoke(nameof(StopWallJumping), wallJumpingDuration); 
        }
  }

        private void StopWallJumping()
        {
            isWallJumping = false;
        }

        public void NiveauSuivants()
        {
             string activeSceneName = SceneManager.GetActiveScene().name;

            if (levelTime < silverTime || levelTime < goldTime)
        {
                 
            if (activeSceneName == "LVL1")
            {
                SceneManager.LoadScene("LVL2");
            }

            if (activeSceneName == "LVL2")
            {
                SceneManager.LoadScene("LVL3");
            }

            if (activeSceneName == "LVL3")
            {
                SceneManager.LoadScene("LVL4");
            }

            if (activeSceneName == "LVL4")
            {
                SceneManager.LoadScene("LVL5");
            }

            if (activeSceneName == "LVL5")
            {
                SceneManager.LoadScene("Menu Principal");
            }
            
        }

        }
        public void Quitter()
    {
        Application.Quit();
    }
       

        
  
        public void Recommancer()
                {
                    string activeSceneName = SceneManager.GetActiveScene().name;

                    SceneManager.LoadScene(activeSceneName);
                }

}
