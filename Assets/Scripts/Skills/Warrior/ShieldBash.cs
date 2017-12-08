using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBash : Skill
{
    [HideInInspector]
    public int StunDuration;

    private void Start()
    {
        Cooldown = 10;
        ManaCost = 30;
        Damage = 10;
        StunDuration = 2;
    }

    public override void Restart()
    {
        Cooldown = 10;
        ManaCost = 30;
        Damage = 10;
        StunDuration = 2;
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
            StunDuration += Mathf.Max(0, UpgradeCost() - 2);

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
            enemy.Stunned = 2;
            enemy.TakeDamage(Damage);
            Player.Instance.animator.SetTrigger("shieldBash");
            SoundManager.Instance.PlaySingle(Player.Instance.sword);

            cooldown = Cooldown;
            GameManager.Instance.Skill1Text.text = cooldown.ToString();
            GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill1Image, 0.1f);
        }

        GameManager.Instance.ChangeTurnDelayed();
    }
}
