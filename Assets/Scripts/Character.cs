using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.Diagnostics;

public abstract class Character : MonoBehaviour
{
    private class MatrixNode
    {
        public MatrixNode(Vector2 position, Vector2 destination)
        {
            this.position = position;
            this.destination = destination;
        }

        public MatrixNode(Vector2 position, Vector2 destination, MatrixNode parent)
        {
            this.position = position;
            this.destination = destination;
            this.parent = parent;
        }

        private double h = -1;
        private double g = 0;

        public double functionF
        {
            get
            {
                return functionG + functionH;
            }
        }

        public double functionG
        {
            get
            {
                if (parent != null)
                {
                    g = parent.functionG;
                    g++;
                }

                return g;
            }
        }

        public double functionH
        {
            get
            {
                if (h == -1)
                    h = Math.Sqrt((destination - position).sqrMagnitude);

                return h;
            }
        }

        public Vector2 position;
        public Vector2 destination;
        public MatrixNode parent;
    }

    protected BoxCollider2D boxCollider;
    protected Rigidbody2D rb2D;
    protected bool moving;

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public float currentHealthPoints;
    [HideInInspector]
    public float currentManaPoints;
    [HideInInspector]
    public int venomTime = 0;
    [HideInInspector]
    public int venomDamage;
    [HideInInspector]
    public int bleedTime = 0;
    [HideInInspector]
    public int bleedDamage;
    [HideInInspector]
    public int disabled = 0;
    [HideInInspector]
    public int slowed = 0;
    [HideInInspector]
    public int Stunned = 0;
    [HideInInspector]
    public int Immobilized = 0;
    [HideInInspector]
    public bool Invisible = false;
    [HideInInspector]
    public SpriteRenderer sr;

    public AudioClip[] shoot;

    public LayerMask blockingLayer;

    protected virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        moving = false;
    }


    private MatrixNode AStar(char[][] board, Vector2 from, Vector2 destination)
    {
        Dictionary<string, MatrixNode> explored = new Dictionary<string, MatrixNode>();
        Dictionary<string, MatrixNode> open = new Dictionary<string, MatrixNode>();
        List<Vector2> neighbors = new List<Vector2>();

        neighbors.Add(new Vector2(-1, 0));
        neighbors.Add(new Vector2(0, 1));
        neighbors.Add(new Vector2(1, 0));
        neighbors.Add(new Vector2(0, -1));

        MatrixNode startNode = new MatrixNode(from, destination);
        string key = from.x.ToString() + from.y.ToString();
        open.Add(key, startNode);

        while (open.Any()) 
        {
            KeyValuePair<string, MatrixNode> smallest = SmallestNode(open);

            foreach (var neighbor in neighbors)
            {
                Vector2 position = smallest.Value.position + neighbor;

                int x = (int)position.x;
                int y = (int)position.y;

                if (x > 0 && x < board.Length && y > 0 && y < board.Length && board[x][y] != 'X')
                {
                    MatrixNode current = new MatrixNode(position, destination, smallest.Value);

                    if (position == destination)
                        return current;

                    key = current.position.x.ToString() + current.position.y.ToString();
                    if (!open.ContainsKey(key) && !explored.ContainsKey(key))
                        open.Add(key, current);
                }
            }

            open.Remove(smallest.Key);
            explored.Add(smallest.Key, smallest.Value);
        }

        return null;
    }


    public bool CanMove(int xDirection, int yDirection, out Vector2 end)
    {
        RaycastHit2D hit;
        Vector2 start = transform.position;
        end = start + new Vector2(xDirection, yDirection);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
            return true;

        return false;
    }

    public bool CanMove(int xDirection, int yDirection)
    {
        RaycastHit2D hit;
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDirection, yDirection);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
            return true;

        return false;
    }


    protected List<Vector2> shortestPath(char[][] matrix, int fromX, int fromY, int toX, int toY)
    {
        Vector2 from = new Vector2(fromX, fromY);
        Vector2 to = new Vector2(toX, toY);

        if (from == to || matrix[toX][toY] == 'X')
            return new List<Vector2>();

        MatrixNode end = AStar(matrix, from, to);

        if (end != null)
        {
            List<Vector2> next = new List<Vector2>();
            Stack<MatrixNode> path = new Stack<MatrixNode>();

            while (end.parent != null)
            {
                path.Push(end);
                end = end.parent;
            }

            path.Push(end);
            path.Pop();

            int tmpX = (int)transform.position.x;
            int tmpY = (int)transform.position.y;

            while (path.Count > 0)
            {
                MatrixNode node = path.Pop();

                int x = (int)node.position.x - tmpX;
                int y = (int)node.position.y - tmpY;

                tmpX = (int)node.position.x;
                tmpY = (int)node.position.y;

                next.Add(new Vector2(x, y));
            }

            return next;
        }

        return new List<Vector2>();
    }

    public float Shoot(Vector2 targetPosition, Rigidbody2D projectile, int damage)
    {
        return Shoot(targetPosition, projectile, damage, 0, 0);
    }

    public float Shoot(Vector2 targetPosition, Rigidbody2D projectile, int damage, int venomDamage, int venomTime)
    {
        Vector2 shootDirection = targetPosition - (Vector2)transform.position;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Rigidbody2D projectileRB = Instantiate(projectile, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
        Projectile projectileScript = projectileRB.GetComponent(typeof(Projectile)) as Projectile;

        projectileScript.damage = damage;
        projectileScript.VenomDamage = venomDamage;
        projectileScript.VenomTime = venomTime;
        projectileScript.source = this;

        shootDirection.Normalize();
        animator.SetTrigger("attack");
        SoundManager.Instance.RandomizeSfx(shoot);
        projectileRB.velocity = new Vector2(shootDirection.x * 10f, shootDirection.y * 10f);

        return angle;
    }

    public void Shoot(Vector2 targetPosition, Rigidbody2D projectile, int damage, float angle)
    {
        Vector2 shootDirection = targetPosition - (Vector2)transform.position;
        Rigidbody2D projectileRB = Instantiate(projectile, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
        Projectile projectileScript = projectileRB.GetComponent(typeof(Projectile)) as Projectile;

        projectileScript.damage = damage;
        projectileScript.source = this;

        shootDirection.Normalize();
        animator.SetTrigger("attack");
        SoundManager.Instance.RandomizeSfx(shoot);
        projectileRB.velocity = new Vector2(shootDirection.x * 10f, shootDirection.y * 10f);
    }

    public void ShootDuplicate(Vector2 targetPosition, Rigidbody2D projectile, int damage)
    {
        Vector2 shootDirection = targetPosition - (Vector2)transform.position;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Rigidbody2D projectileRB = Instantiate(projectile, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
        Projectile projectileScript = projectileRB.GetComponent(typeof(Projectile)) as Projectile;

        projectileScript.damage = damage;
        projectileScript.VenomDamage = 0;
        projectileScript.VenomTime = 0;
        projectileScript.source = this;
        projectileScript.duplicate = true;

        shootDirection.Normalize();
        animator.SetTrigger("attack");
        SoundManager.Instance.RandomizeSfx(shoot);
        projectileRB.velocity = new Vector2(shootDirection.x * 10f, shootDirection.y * 10f);
    }

    private KeyValuePair<string, MatrixNode> SmallestNode(Dictionary<string, MatrixNode> open)
    {
        KeyValuePair<string, MatrixNode> smallest = open.ElementAt(0);

        foreach (KeyValuePair<string, MatrixNode> item in open)
        {
            if (item.Value.functionF < smallest.Value.functionF)
                smallest = item;
        }

        return smallest;
    }
}
