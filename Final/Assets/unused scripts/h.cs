using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class h : MonoBehaviour
{

    [SerializeField] private float startinHealth;
    public float currenHealth { get; private set; }
    public float maxHealth = 10;
    public int health;
    public Animator anim;
    private bool Die;
    public static bool move;
    public AudioSource src;
    //private int Sen = 0;


    private void Start()
    {
        move = true;
    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("player2"))
        {
            anim.Play("Hurt");

        }
    }


    private void Awake()
    {
        currenHealth = startinHealth;
        anim = GetComponent<Animator>();
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //SceneManager.LoadScene("BossLevel1");
    // Sen = 1;

    // }



    public void TackDamage(float damage)
    {
        currenHealth = Mathf.Clamp(currenHealth - damage, 0, startinHealth);
        currenHealth -= damage;
        if (currenHealth <= 0)
        {
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);











        }
        //else if(currenHealth <= 0 && Sen == 1)
        // {
        //  Destroy(gameObject);
        //SceneManager.LoadScene("BossLevel1");


        //}


        if (currenHealth <= 0)
        {
            move = false;
        }





        if (currenHealth > 0)
        {
            anim.SetTrigger("Hurt");
        }
        else
        {
            if (!Die)
            {
                anim.SetTrigger("Death");
                GetComponent<heroknight2>().enabled = false;
                Die = true;



            }

        }


    }


    public void AddHealth(float value)
    {
        currenHealth = Mathf.Clamp(currenHealth + value, 0, startinHealth);
        src.Play();
    }

    void Restart()
    {
        SceneManager.LoadScene(0);
    }

}
