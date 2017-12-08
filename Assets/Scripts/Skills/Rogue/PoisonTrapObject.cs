using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTrapObject : MonoBehaviour
{
    [HideInInspector]
    public int damage;
    [HideInInspector]
    public int venomTime;
    [HideInInspector]
    public int venomDamage;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag.Equals("Enemy"))
        {
            GenericEnemy enemy = collision.GetComponent<GenericEnemy>();
            enemy.TakeDamage(damage);
            enemy.venomTime = venomTime;
            enemy.venomDamage = venomDamage;

            Destroy(gameObject);
        }
    }
}
