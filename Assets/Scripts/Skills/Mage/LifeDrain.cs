using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeDrain : Skill
{
    [HideInInspector]
    public int Heal;

    public Rigidbody2D projectile;

    public override void Restart()
    {
        Cooldown = 10;
        ManaCost = 25;
        Damage = 30;
        Heal = 20;
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
            Damage += 10;
            Heal += 5;

            GameManager.Instance.ShowSkills();
        }
    }

    public override void Use(GenericEnemy enemy)
    {
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost || !Player.Instance.CanShoot(enemy.transform.position)) return;

        Player.Instance.currentManaPoints -= ManaCost;
        Player.Instance.UpdatePlayerMana();

        Player.Instance.animator.SetTrigger("attack");
        Player.Instance.Shoot(enemy.transform.position, projectile, Player.Instance.damage + Damage);
        Player.Instance.currentHealthPoints += Heal;

        if (Player.Instance.currentHealthPoints > Player.Instance.properties.HealthPoints)
            Player.Instance.currentHealthPoints = Player.Instance.properties.HealthPoints;

        Player.Instance.UpdatePlayerHealth();

        cooldown = Cooldown;
        GameManager.Instance.Skill2Text.text = cooldown.ToString();
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill2Image, 0.1f);
        Player.Instance.BlockUpdate();
    }
}
