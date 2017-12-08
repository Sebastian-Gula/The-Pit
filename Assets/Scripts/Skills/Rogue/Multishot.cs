using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Multishot : Skill
{
    [HideInInspector]
    public int Arrows;


    public override void Restart()
    {
        Cooldown = 8;
        ManaCost = 25;
        Damage = 0;
        Arrows = 2;
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
            Arrows += Lvl % 2 == 1 ? 2 : 0;

            GameManager.Instance.ShowSkills();
        }
    }

    public override void Use(GenericEnemy enemy)
    {
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost || !Player.Instance.CanShoot(enemy.transform.position)) return;

        Player.Instance.currentManaPoints -= ManaCost;
        Player.Instance.UpdatePlayerMana();    
        Player.Instance.animator.SetTrigger("attack");

        Vector2 distance = (Vector2)enemy.transform.position - (Vector2)Player.Instance.transform.position;
        float r = distance.sqrMagnitude;
        float angle = Player.Instance.Shoot((Vector2)enemy.transform.position, Player.Instance.arrow, Player.Instance.damage);

        for (int i = 0; i < Arrows / 2; i++)
        {
            float x = Player.Instance.transform.position.x + (float)Math.Cos((angle + (50 * (i + 1))) * Math.PI / 180) * r;
            float y = Player.Instance.transform.position.y + (float)Math.Sin((angle + (50 * (i + 1))) * Math.PI / 180) * r;

            Player.Instance.Shoot(new Vector2(x, y), Player.Instance.arrow, Player.Instance.damage);
        }

        for (int i = 0; i < Arrows / 2; i++)
        {
            float x = Player.Instance.transform.position.x + (float)Math.Cos((angle - (50 * (i + 1))) * Math.PI / 180) * r;
            float y = Player.Instance.transform.position.y + (float)Math.Sin((angle - (50 * (i + 1))) * Math.PI / 180) * r;

            Player.Instance.Shoot(new Vector2(x, y), Player.Instance.arrow, Player.Instance.damage);
        }

        cooldown = Cooldown;
        GameManager.Instance.Skill2Text.text = cooldown.ToString();
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill2Image, 0.1f);
        Player.Instance.BlockUpdate();
    }
}
