using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator Anim;
    [SerializeField] private float Speed, JumpForce, groundCheckRadius, SuricaneRotation, SuricaneForce, damageTime;
    [SerializeField] private Transform groundChecker;
    [SerializeField] private LayerMask WhatIsGround;
    [SerializeField] private GameObject Suricane;
    [SerializeField] private Transform firePoint;
    [SerializeField] private AudioSource stepsSound, jumpSound, suricaneSound;
    [SerializeField] private AudioClip jumpClip, LandingClip;
    [SerializeField] private AudioClip[] stepClips;
    [SerializeField] private TextMeshProUGUI ScoreText;

    private int score;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private float moveX, damagedTimer;
    private bool grounded, landing;
    private float firePointX;

    private bool damaged;

    void Start()
    {
        damaged = false;
        rb = GetComponent<Rigidbody2D>();
        sp = Anim.gameObject.GetComponent<SpriteRenderer>();
        firePointX = firePoint.localPosition.x;
    }

    void Update()
    {
        if(damagedTimer > 0)
        {
            damagedTimer -= Time.deltaTime;
        }
        else
        {
            damaged = false;
            sp.color = Color.white;
        }

        landing = grounded;

        moveX = Input.GetAxis("Horizontal");
        grounded = Physics2D.OverlapCircle(groundChecker.position, groundCheckRadius, WhatIsGround);

        if(grounded == true && landing == false)
        {
            jumpSound.clip = LandingClip;
            jumpSound.volume = 0.4f;
            jumpSound.Play();

        }

        Vector2 velo = Vector2.zero;
        velo.x = moveX * Speed;
        velo.y = rb.velocity.y;

        rb.velocity = velo;

        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            jumpSound.clip = jumpClip;
            jumpSound.volume = 1;
            jumpSound.Play();
        }

        StartCoroutine(FireCheck());
        Flip();
        AnimatorCheck();
    }
    void AnimatorCheck()
    {
        Anim.SetFloat("Velocity", Mathf.Abs(rb.velocity.x));
        Anim.SetFloat("VelocityY", rb.velocity.y);
        Anim.SetBool("Grounded", grounded);
    }

    IEnumerator FireCheck()
    {
        
        if (Input.GetMouseButtonDown(0) && grounded)
        {
            Anim.SetTrigger("Attack"); 
            
            yield return new WaitForSeconds(0.2f);

            Suricane suricane = Instantiate(Suricane, firePoint.position, Quaternion.identity).GetComponent<Suricane>();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 force = mousePos - firePoint.position;
            suricane.SetForceAndRotation(force.normalized * SuricaneForce, SuricaneRotation);
            suricaneSound.Play();
        }
    }

    void Flip()
    {
        if (moveX > 0)
        {
            sp.flipX = false;
            firePoint.localPosition = new Vector3(firePointX, firePoint.localPosition.y, 0);
        }
        else if (moveX < 0)
        {
            sp.flipX = true;
            firePoint.localPosition = new Vector3(-firePointX, firePoint.localPosition.y, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Coin"))
        {
            score++;
            ScoreText.text = "Glyphs: " + score;
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy") && !damaged)
        {
            damagedTimer = damageTime;
            damaged = true;
            sp.color = new Color(1, 0.5f, 0.5f, 0.5f);
        }
    }

    public void playRandomStep()
    {
        stepsSound.clip = stepClips[Random.Range(0, 4)];
        stepsSound.Play();
    }

}
