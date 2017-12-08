using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sacrifice : Skill
{
    [HideInInspector]
    public int HealthCost;


    public override void Restart()
    {
        Cooldown = 20;
        ManaCost = 0;
        HealthCost = 40;
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
 
            GameManager.Instance.ShowSkills();
        }
    }

    public override void Use(GenericEnemy enemy)
    {
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost) return;
        
        Player.Instance.currentHealthPoints -= HealthCost;
        Player.Instance.currentManaPoints = Player.Instance.properties.ManaPoints;
        Player.Instance.Thunderbolt.cooldown = 0;
        Player.Instance.PoisonMissile.cooldown = 0;
        Player.Instance.Glyph.cooldown = 0;
        Player.Instance.LifeDrain.cooldown = 0;
        Player.Instance.UpdatePlayerHealth();
        Player.Instance.UpdatePlayerMana();

        cooldown = Cooldown;
        GameManager.Instance.Skill1Text.text = "";
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill1Image, 1);

        GameManager.Instance.Skill2Text.text = "";
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill2Image, 1);
 
        GameManager.Instance.Skill3Text.text = cooldown.ToString();
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill3Image, 0.1f);
        GameManager.Instance.ChangeTurnDelayed();
    }
}
