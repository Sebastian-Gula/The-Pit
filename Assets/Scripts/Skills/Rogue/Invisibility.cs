using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisibility : Skill
{
    [HideInInspector]
    public float damageAmplifier;
    [HideInInspector]
    public int turns;
    [HideInInspector]
    public int Turns;


    public void DecreaseTurns()
    {
        if (turns > 0)
        {
            turns--;

            if (turns == 0)
            {
                Player.Instance.Invisible = false;
                Player.Instance.damageAmplifier = 1;

                Color tmpColor = Player.Instance.sr.color;
                tmpColor.a = 1;

                Player.Instance.sr.color = tmpColor;
            }

        }
    }


    public override void Restart()
    {
        Cooldown = 10;
        ManaCost = 25;
        Damage = 0;
        damageAmplifier = 1.5f;
        Turns = 5;
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
            damageAmplifier += 0.1f;
            Turns += Lvl % 2;

            GameManager.Instance.ShowSkills();
        }
    }

    public override void Use(GenericEnemy enemy)
    {
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost) return;

        Player.Instance.currentManaPoints -= ManaCost;
        Player.Instance.UpdatePlayerMana();

        Player.Instance.Invisible = true;
        Player.Instance.damageAmplifier = damageAmplifier;
        turns = Turns;

        cooldown = Cooldown;
        GameManager.Instance.Skill2Text.text = cooldown.ToString();
        GameManager.Instance.ChangeAlphaOfImage(Player.Instance.sr, 0.4f);
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill2Image, 0.1f);
        GameManager.Instance.ChangeTurnDelayed();
    }
}
