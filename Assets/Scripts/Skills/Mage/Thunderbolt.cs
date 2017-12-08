using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunderbolt : Skill
{
    public Rigidbody2D thunderbolt;


    public override void Restart()
    {
        Cooldown = 10;
        ManaCost = 20;
        Damage = 40;
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
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost || !Player.Instance.CanShoot(enemy.transform.position)) return;

        Player.Instance.currentManaPoints -= ManaCost;
        Player.Instance.UpdatePlayerMana();
        Player.Instance.animator.SetTrigger("attack");
        Player.Instance.Shoot((Vector2)enemy.transform.position, thunderbolt, Player.Instance.damage + Damage);

        cooldown = Cooldown;
        GameManager.Instance.Skill1Text.text = cooldown.ToString();
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill1Image, 0.1f);
        Player.Instance.BlockUpdate();
    }
}
