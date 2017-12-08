using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class GenericEnemy : Character
{
    public enum Type { Meele, Ranged }

    private List<Vector2> destination = new List<Vector2>();
    private Image HealthBar;

    public static bool lastEnemy;
    public static bool stopPlayerMovment = true;

    public CharacterProperties properties;
    public int exp;
    public int damage;
    public int armor;
    public int movesPerRound;
    public Type type;

    public AudioClip[] zombie;
    public AudioClip[] skeleton;

    public Image HealthBarPref;
    public Rigidbody2D projectile;


    protected override void Awake()
    {
        base.Awake();
        enabled = false;

        if(GameManager.Instance.level > 1)
        {
            int bonusHealth = 20;
            int bonusExp    = 0;
            int bonusDamage = 0;

            for (int i = 1; i < GameManager.Instance.level; i++)
            {
                bonusHealth += properties.HealthPoints / 10 * i;
                bonusExp += exp / 10 * i;
                bonusDamage += damage / 5 * i;          
            }            

            properties.HealthPoints += bonusHealth;
            exp += bonusExp;
            damage += bonusDamage;
            armor += GameManager.Instance.level;

        }

        currentHealthPoints = properties.HealthPoints;
        currentManaPoints = properties.ManaPoints;
        name = "GenericEnemy";

        BoardManager.obstacles[(int)transform.position.x][(int)transform.position.y] = 'X';

        GameManager.Instance.enemies.Add(this);
    }

    public virtual int Attack()
    {
        if (type == Type.Meele && CanNormalAttack())
        {
            NormalAttack();

            if (UnityEngine.Random.Range(0, 100) > 60)
                SoundManager.Instance.RandomizeSfx(zombie);

            return 1;
        }
        else if (type == Type.Ranged && CanShoot(Player.Instance.transform.position))
        {
            Vector2 playerPosition;

            if (GameManager.Instance.DecoyIsUp)
                playerPosition = DecoyObject.Instance.transform.position;
            else
                playerPosition = Player.Instance.transform.position;

            Shoot(playerPosition, projectile, damage);

            return 1;
        }

        return 0;
    }


    protected bool CanNormalAttack()
    {
        if (gameObject != null)
        {
            Vector2 playerPosition;

            if (GameManager.Instance.DecoyIsUp)
                playerPosition = DecoyObject.Instance.transform.position;
            else
                playerPosition = Player.Instance.transform.position;

            Vector2 distance = (Vector2)transform.position - playerPosition;
            float sqrDistance = distance.sqrMagnitude;

            return sqrDistance <= 2;
        }
        else
            return false;
    }

    protected bool CanShoot(Vector2 targetPosition)
    {
        return CanShoot(transform.position, targetPosition);
    }

    protected bool CanShoot(Vector2 shootPosition, Vector2 targetPosition)
    {
        if (this != null)
        {
            RaycastHit2D hit;
            int x = (int) Math.Abs(shootPosition.x - targetPosition.x);
            int y = (int)Math.Abs(shootPosition.y - targetPosition.y);

            if (x < 7 && y < 5)
            {
                boxCollider.enabled = false;
                hit = Physics2D.Linecast(shootPosition, targetPosition, blockingLayer);
                boxCollider.enabled = true;

                if (hit.collider != null && (hit.collider.tag.Equals("Hero") || hit.collider.tag.Equals("Decoy")))
                    return true;
            }
        }

        return false;
    }

    public IEnumerator DoMoving(List<Vector2> destination)
    {
        moving = true;
        Vector2 end;

        while (destination.Count > 0 && Immobilized == 0)
        {
            if (CanMove((int)destination[0].x, (int)destination[0].y, out end))
            {
                float sqrRemainingDistance = ((Vector2)transform.position - end).sqrMagnitude;

                while (sqrRemainingDistance > float.Epsilon)
                {
                    Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, 4.5f * Time.deltaTime);
                    rb2D.MovePosition(newPostion);
                    sqrRemainingDistance = ((Vector2)transform.position - end).sqrMagnitude;

                    yield return null;
                }
            }

            destination.RemoveAt(0);

            yield return new WaitForSeconds(0.1f);
        }

        BoardManager.obstacles[(int)transform.position.x][(int)transform.position.y] = 'X';

        moving = false;
    }


    public Vector2 NearestPosition(int layer, Vector2 playerPosition)
    {
        Vector2 enemyPosition = transform.position;
        Vector2 nearestPosition = new Vector2();   
        Dictionary<Vector2, float> possibleLocation = new Dictionary<Vector2, float>();

        float min = Mathf.Infinity;
        float distance;

        int sx = (int)playerPosition.x - layer;
        int ex = (int)playerPosition.x + layer;
        int sy = (int)playerPosition.y - layer;
        int ey = (int)playerPosition.y + layer;

        for (int x = sx; x <= ex; x++)
        {
            for (int y = sy; y <= ey; y++)
            {
                if(BoardManager.obstacles[x][y] == '-' && (x == sx || x == ex || y == sy|| y == ey))
                {
                    if (type == Type.Meele)
                    {
                        Vector2 potentialNearestPosition = new Vector2(x, y);
                        distance = (enemyPosition - potentialNearestPosition).sqrMagnitude;

                        if (distance < min)
                        {
                            min = distance;
                            nearestPosition = potentialNearestPosition;
                        }
                    }
                    else if (type == Type.Ranged && CanShoot(new Vector2(x, y), playerPosition))
                    {
                        Vector2 position = new Vector2(x, y);

                        possibleLocation.Add(position, (enemyPosition - position).sqrMagnitude);
                    }
                }
            }
        }
        
        if(possibleLocation.Any())
            return possibleLocation.OrderBy(kvp => kvp.Value).First().Key;

        if(min == Mathf.Infinity)
        {
            if (layer < 3)
            {
                int nextLayer = ++layer;
                nearestPosition = NearestPosition(nextLayer, playerPosition);
            }            
            else
                nearestPosition = enemyPosition;
        }   

        return nearestPosition;
    }


    public int Move()
    {
        Vector2 playerPosition;

        if (GameManager.Instance.DecoyIsUp)
            playerPosition = DecoyObject.Instance.transform.position;
        else
            playerPosition = Player.Instance.transform.position;

        float distance = ((Vector2)transform.position - playerPosition).sqrMagnitude;

        if (type == Type.Meele && distance <= 2)
            return 0;
        else if (type == Type.Ranged && distance >= 4 && CanShoot(playerPosition))
            return 0;

        Vector2 freePosition;

        if (type == Type.Ranged)
            freePosition = NearestPosition(3, playerPosition);
        else
            freePosition = NearestPosition(1, playerPosition);

        BoardManager.obstacles[(int)transform.position.x][(int)transform.position.y] = '-';

        try
        {
            destination = shortestPath(BoardManager.obstacles, (int)transform.position.x, (int)transform.position.y, (int)freePosition.x, (int)freePosition.y);
        }
        catch (NullReferenceException) { return 0; }

        if (destination.Count > movesPerRound)
            destination.RemoveRange(movesPerRound, destination.Count - movesPerRound);

        if (UnityEngine.Random.Range(0, 100) > 80)
        {
            if (type == Type.Meele)
                SoundManager.Instance.RandomizeSfx(zombie);
            else if (type == Type.Ranged)
                SoundManager.Instance.RandomizeSfx(skeleton);
        }

        StartCoroutine("DoMoving", destination);

        return destination.Count;
    }


    protected virtual void NormalAttack()
    {
        animator.SetTrigger("attack");

        if (GameManager.Instance.DecoyIsUp)
            DecoyObject.Instance.TakeDamage(damage);
        else
            Player.Instance.TakeDamage(damage);
    }


    private void OnEnable()
    {
        lastEnemy = false;

        if (stopPlayerMovment)
        {
            Player.Instance.StopMoving();
            stopPlayerMovment = false;
        }

        Transform objectsHolder = GameManager.Instance.InGameUI.transform;
        HealthBar = Instantiate(HealthBarPref, new Vector3(transform.position.x, transform.position.y + 0.6f, 0f), Quaternion.identity) as Image;
        HealthBar.transform.SetParent(objectsHolder, false);
    }


    public void TakeDamage(float damage)
    {
        if(this != null)
        {
            if (animator != null)
                animator.SetTrigger("damage");

            damage *= Player.Instance.damageAmplifier;

            if (type == Type.Meele)
                SoundManager.Instance.RandomizeSfx(zombie);
            else if (type == Type.Ranged)
                SoundManager.Instance.RandomizeSfx(skeleton);

            currentHealthPoints -= damage;
            HealthBar.fillAmount = currentHealthPoints / properties.HealthPoints;

            if (currentHealthPoints <= 0)
            {
                Player.Instance.EndGameInfo.EnemiesKilled++;
                BoardManager.obstacles[(int)transform.position.x][(int)transform.position.y] = '-';
                GameManager.Instance.enemies.Remove(this);
                Player.Instance.GainExp(exp);

                lastEnemy = !GameManager.Instance.enemies.Any(enemy => enemy.enabled);

                if (lastEnemy)
                {
                    GameManager.Instance.EndFight();
                    stopPlayerMovment = true;
                }                 

                Destroy(gameObject);
            }
        }
    }


    private void Update()
    {
        if (HealthBar != null)
            HealthBar.transform.position = transform.position + new Vector3(0, 0.6f, 0);
    }
}
