using UnityEngine;

public class Projectile : MonoBehaviour
{
    private bool targetHit = false;

    [HideInInspector]
    public int damage = 0;
    [HideInInspector]
    public int VenomDamage;
    [HideInInspector]
    public int VenomTime;
    [HideInInspector]
    public Character source;
    [HideInInspector]
    public bool duplicate = false;

    public void Awake()
    {
        Invoke("DestroyThisObject", 2);
    }


    public void DestroyThisObject()
    {
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Enemy") && !source.tag.Equals("Enemy"))
        {
            targetHit = !targetHit;

            if (targetHit)
            {
                GenericEnemy enemy = other.GetComponent<GenericEnemy>();

                if (enemy.enabled)
                {
                    enemy.TakeDamage(damage);

                    if (!duplicate)
                        GameManager.Instance.ChangeTurnDelayed();

                    if (VenomTime > 0 && VenomDamage > 0)
                    {
                        enemy.venomTime = VenomTime;
                        enemy.venomDamage = VenomDamage;
                    }
                }
            }

            Destroy(gameObject);
        }
        else if (other.tag.Equals("Hero") && !source.tag.Equals("Hero"))
        {
            Player.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.tag.Equals("Decoy"))
        {
            DecoyObject.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }    
    }
}
