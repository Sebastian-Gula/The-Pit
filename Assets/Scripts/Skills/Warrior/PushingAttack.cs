using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingAttack : Skill {

    public override void Restart()
    {
        Cooldown = 10;
        ManaCost = 20;
        Damage =  20;
        Lvl = 1;
    }


    public override void Upgrade()
    {
        if (Player.Instance.skillPoints >= UpgradeCost() && Lvl < 5)
        {
            Player.Instance.skillPoints -= UpgradeCost();

            Lvl++;
            Cooldown -= Lvl % 2;
            ManaCost -= 1;
            Damage += 15;

            GameManager.Instance.ShowSkills();
        }
    }

    public override void Use(GenericEnemy enemy)
    {
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost) return;

        Player.Instance.currentManaPoints -= ManaCost;
        Player.Instance.UpdatePlayerMana();

        Vector2 targetPosition = enemy.transform.position;
        Vector2 distance = targetPosition - (Vector2)Player.Instance.transform.position;
        Vector2 end;
        float sqrDistance = distance.sqrMagnitude;

        if (sqrDistance <= 2)
        {         
            enemy.TakeDamage(Damage + Player.Instance.damage);
            Player.Instance.animator.SetTrigger("pushingAttack");
            SoundManager.Instance.PlaySingle(Player.Instance.sword);
            
            List<Vector2> destination = new List<Vector2>();

            if (distance == new Vector2(0, 1) || distance == new Vector2(1, 1))
                destination.Add(new Vector2(0, 1));
            else if (distance == new Vector2(1, 0) || distance == new Vector2(1, -1))
                destination.Add(new Vector2(1, 0));
            else if (distance == new Vector2(0, -1) || distance == new Vector2(-1, -1))
                destination.Add(new Vector2(0, -1));
            else if (distance == new Vector2(-1, 0) || distance == new Vector2(-1, 1))
                destination.Add(new Vector2(-1, 0));

            if (enemy.CanMove((int)destination[0].x, (int)destination[0].y, out end))
            {
                BoardManager.obstacles[(int)enemy.transform.position.x][(int)enemy.transform.position.y] = '-';
                enemy.StartCoroutine("DoMoving", destination);
            }
            else
                enemy.TakeDamage(Damage + Player.Instance.damage);

            cooldown = Cooldown;
            GameManager.Instance.Skill2Text.text = cooldown.ToString();
            GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill2Image, 0.1f);
        }

        GameManager.Instance.ChangeTurnDelayed();
    }
}
