using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WideAttack : Skill
{
    public override void Restart()
    {
        Cooldown = 10;
        ManaCost = 20;
        Damage = 0;
        Lvl = 1;
    }


    public override void Upgrade()
    {
        if (Player.Instance.skillPoints >= UpgradeCost() && Lvl < 5)
        {
            Player.Instance.skillPoints -= UpgradeCost();

            Lvl++;
            Cooldown -= Lvl % 2;
            ManaCost -= 2;
            Damage += 10;

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
        float sqrDistance = distance.sqrMagnitude;

        if (sqrDistance <= 2)
        {         
            enemy.TakeDamage(Damage + Player.Instance.damage);
            Player.Instance.animator.SetTrigger("wideAttack");
            SoundManager.Instance.PlaySingle(Player.Instance.sword);
            
            List<Vector2> possibleEnemiesLocations = new List<Vector2>();

            if (distance == new Vector2(0, 1) || distance == new Vector2(0, -1))
            {
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(-1, 0));
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(1, 0));
            }
            else if (distance == new Vector2(1, 0) || distance == new Vector2(-1, 0))
            {
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(0, -1));
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(0, 1));
            }
            else if (distance == new Vector2(1, 1))
            {
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(-1, 0));
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(0, -1));
            }
            else if (distance == new Vector2(1, -1))
            {
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(0, 1));
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(-1, 0));
            }
            else if (distance == new Vector2(-1, -1))
            {
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(0, 1));
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(1, 0));
            }
            else if (distance == new Vector2(-1, 1))
            {
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(1, 0));
                possibleEnemiesLocations.Add((Vector2)enemy.transform.position + new Vector2(0, -1));
            }

            foreach (var possibleEnemy in GameManager.Instance.sortedEnemies)
            {
                if(possibleEnemiesLocations.Contains(possibleEnemy.Key.transform.position))
                {
                    possibleEnemy.Key.TakeDamage(Damage + Player.Instance.damage);
                }
            }

            cooldown = Cooldown;
            GameManager.Instance.Skill1Text.text = cooldown.ToString();
            GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill1Image, 0.1f);
        }

        GameManager.Instance.ChangeTurnDelayed();
    }
}
