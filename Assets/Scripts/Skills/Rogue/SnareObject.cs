using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnareObject : MonoBehaviour
{
    [HideInInspector]
    public int damage;
    [HideInInspector]
    public int Immobilize;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag.Equals("Enemy"))
        {
            GenericEnemy enemy = collision.GetComponent<GenericEnemy>();
            enemy.Immobilized = Immobilize;
            enemy.TakeDamage(damage); 

            Destroy(gameObject);
        }
    }
}