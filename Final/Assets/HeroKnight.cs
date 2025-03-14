﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroKnight : MonoBehaviour {

    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    //[SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;

    public int maxHealth = 100;
    int currentHealth;
    int currentH = 200;
    bool block = false;
    bool facingRight = true;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemylayer;
    public int attackDamage = 20;
    public Text text;
    public SpriteRenderer img1;
    public SpriteRenderer img2;
    [SerializeField] private GameObject again;
    [SerializeField] private GameObject mainmenu;
    private AudioSource m_audioSource;
    private AudioManager_PrototypeHero m_audioManager;
    public AudioSource gameend;

    // Use this for initialization
    void Start ()
    {
        gameend.enabled = false;
        m_audioSource = GetComponent<AudioSource>();
        m_audioManager = AudioManager_PrototypeHero.instance;
        again.SetActive(false);
        mainmenu.SetActive(false);
        img1.enabled = false;
        img2.enabled = false;
        currentHealth = maxHealth;

        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown("q") && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            Attack();
        }

        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if(m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if(m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0 && !facingRight)
        {
            //GetComponent<SpriteRenderer>().flipX = false;
            Flip();
            block = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0 && facingRight)
        {
            //GetComponent<SpriteRenderer>().flipX = true;
            Flip();
            block = false;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling )
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Death
        //if (Input.GetKeyDown("e") && !m_rolling)
        //{
        //    m_animator.SetBool("noBlood", m_noBlood);
        //    m_animator.SetTrigger("Death");
        //}

        //Hurt
        //else if (Input.GetKeyDown("q") && !m_rolling)
        //    m_animator.SetTrigger("Hurt");
        // if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)

        //Attack
        //if (Input.GetKeyDown("q") && m_timeSinceAttack > 0.25f && !m_rolling)
        //{
        //    m_currentAttack++;
        
            // Loop back to one after third attack
        //    if (m_currentAttack > 3)
        //        m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
        //    if (m_timeSinceAttack > 1.0f)
        //        m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
        //    m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
        //    m_timeSinceAttack = 0.0f;
        //}

        // Block
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
            block = true;
            Invoke("unblock", 1f);
        }

        else if (Input.GetKeyDown("e"))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }
            

        //Jump
        else if (Input.GetKeyDown("s") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    public void TakeDamage(int damage)
    {
        if(block == false)
        {
            currentHealth -= damage;
            m_animator.SetTrigger("Hurt");

            if(currentHealth <= 0)
            {
                Die();
            }
        }
        
    }

    void Die()
    {
        Debug.Log("Enemy died!");

        m_animator.SetTrigger("Death");

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        GameObject blur = GameObject.FindGameObjectWithTag("blur");
        blur.GetComponent<Krivodeling.UI.Effects.UIBlur>().BeginBlur(10000f);

        img1.enabled = true;
        img2.enabled = true;
        again.SetActive(true);
        mainmenu.SetActive(true);
        gameend.enabled = true;
    }

    private void unblock()
    {
        block = false;
    }

    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }

    void Attack()
    {
        //if (Input.GetKeyDown("o") && m_timeSinceAttack > 0.25f && !m_rolling)
        //{
        m_currentAttack++;

        // Loop back to one after third attack
        if (m_currentAttack > 3)
            m_currentAttack = 1;

        // Reset Attack combo if time since last attack is too large
        if (m_timeSinceAttack > 1.0f)
            m_currentAttack = 1;

        // Call one of three attack animations "Attack1", "Attack2", "Attack3"
        m_animator.SetTrigger("Attack" + m_currentAttack);

        // Reset timer
        m_timeSinceAttack = 0.0f;

        Collider2D[] hithero = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemylayer);

        foreach (Collider2D enemy in hithero)
        {
            enemy.GetComponent<heroknight2>().TakeDamage2(attackDamage);
            enemy.GetComponent<heroknight2>().chealth(attackDamage);
            //Debug.Log("We hit " + enemy.name);
        }
        //}
    }

    public void chealth(int damage)
    {
        if (block == false)
        {
            currentH -= damage;

            text.text = currentH + "/200";

            if (currentH == 180)
            {
                var block = GameObject.FindWithTag("1");
                Destroy(block);
            }
            if (currentH == 160)
            {
                var block = GameObject.FindWithTag("2");
                Destroy(block);
            }
            if (currentH == 140)
            {
                var block = GameObject.FindWithTag("3");
                Destroy(block);
            }
            if (currentH == 120)
            {
                var block = GameObject.FindWithTag("4");
                Destroy(block);
            }
            if (currentH == 100)
            {
                var block = GameObject.FindWithTag("5");
                Destroy(block);
            }
            if (currentH == 80)
            {
                var block = GameObject.FindWithTag("6");
                Destroy(block);
            }
            if (currentH == 60)
            {
                var block = GameObject.FindWithTag("7");
                Destroy(block);
            }
            if (currentH == 40)
            {
                var block = GameObject.FindWithTag("8");
                Destroy(block);
            }
            if (currentH == 20)
            {
                var block = GameObject.FindWithTag("9");
                Destroy(block);
            }
            if (currentH == 0)
            {
                var block = GameObject.FindWithTag("10");
                Destroy(block);
            }
        }
    }

    void AE_footstep()
    {
        m_audioManager.PlaySound("Footstep");
    }

    void AE_Jump()
    {
        m_audioManager.PlaySound("Jump");
    }

    void AE_Landing()
    {
        m_audioManager.PlaySound("Landing");
    }

    void AE_runStop()
    {
        m_audioManager.PlaySound("RunStop");
    }

    void AE_Hurt()
    {
        m_audioManager.PlaySound("Hurt");
    }
}
