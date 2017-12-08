using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyObject : MonoBehaviour {

    public static DecoyObject Instance;
    [HideInInspector]
    public float health;
    [HideInInspector]
    public float turns;


    public void Awake()
    {
        Instance = this;
    }


    public void DecreaseTurns()
    {
        if (turns > 0)
        {
            turns--;

            if (turns == 0)
            {
                Destroy(gameObject);
                GameManager.Instance.DecoyIsUp = false;
            }
        }
    }


    public void TakeDamage(float damage)
    {
        damage *= 2;
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
            GameManager.Instance.DecoyIsUp = false;
        }
    }
}
