using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed, jumpForce, groundCheckRadius;
    [SerializeField] private GameObject spriteGO;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private LayerMask ground;
    
    private Animator anim;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private float moveX, moveY;
    private bool grounded;

    public LayerMask enemyLayer;
    
    void Start()
    {
        anim = spriteGO.GetComponent<Animator>();
        sprite = spriteGO.GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        if (moveX == -1)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
            //spriteGO.GetComponent<SpriteRenderer>().flipX = true;        
        }

        if (moveX == 1)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            //spriteGO.GetComponent<SpriteRenderer>().flipX = false;
        }

        grounded = Physics2D.OverlapCircle(groundChecker.position, groundCheckRadius, ground);
        Vector2 velo = Vector2.zero;
        velo.x = moveX * speed;
        velo.y = rb.velocity.y;
        rb.velocity = velo;
        if (Input.GetKeyDown("w") && grounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        if (Input.GetButtonDown("FireBySpace") && grounded)
        {
            Attack();
        }
        CheckAnimation();
    }

    //TODO find some other realization for control hit
    private void Attack()
    {
        anim.SetTrigger("Attack");
    }

    private void CheckAnimation()
    {
        anim.SetBool("Grounded", grounded);

        anim.SetFloat("VelocityX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("VelocityY", rb.velocity.y);
    }
}
