using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class h2 : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 0f;
    public LayerMask enemyLayers;
    public Animator animator;

    public int attackDamage = 0;
    public float attackRate = 0f;
    float nextattacktime = 0.5f;

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextattacktime)
        {
            if (Input.GetMouseButton(0))
            {
                Attack();
                nextattacktime = Time.time + 0.5f / attackRate;

            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);


        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<h>().TackDamage(attackDamage);
        }
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<h3>().TackDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected2()
    {
        if (attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
