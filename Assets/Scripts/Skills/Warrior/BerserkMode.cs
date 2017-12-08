using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkMode : Skill
{
    [HideInInspector]
    public float damageAmplifier;
    [HideInInspector]
    public float defenceAmplifier;
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
                if (Player.Instance.weapon.WeponName.Equals("One handed sword"))
                    Player.Instance.animator.runtimeAnimatorController = Player.Instance.warrior1Controller;
                else if (Player.Instance.weapon.WeponName.Equals("Two handed sword"))
                    Player.Instance.animator.runtimeAnimatorController = Player.Instance.warrior2Controller;

                Player.Instance.damageAmplifier = 1;
                Player.Instance.defenceAmplifier = 1;
            }
        }
    }


    public override void Restart()
    {
        Cooldown = 20;
        ManaCost = 50;
        Damage = 0;
        damageAmplifier = 1.2f;
        defenceAmplifier = 1.2f;
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
            damageAmplifier += 0.2f;
            defenceAmplifier += 0.2f;
            Turns += Lvl % 2;

            GameManager.Instance.ShowSkills();
        }
    }

    public override void Use(GenericEnemy enemy)
    {
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost) return;

        Player.Instance.currentManaPoints -= ManaCost;
        Player.Instance.UpdatePlayerMana();
        Player.Instance.damageAmplifier = damageAmplifier;
        Player.Instance.defenceAmplifier = defenceAmplifier;
        turns = Turns;   
        
        if (Player.Instance.weapon.WeponName.Equals("One handed sword"))
            Player.Instance.animator.runtimeAnimatorController = Player.Instance.warrior1BerserkerController;
        else if (Player.Instance.weapon.WeponName.Equals("Two handed sword"))
            Player.Instance.animator.runtimeAnimatorController = Player.Instance.warrior2BerserkerController;

        cooldown = Cooldown;
        GameManager.Instance.Skill3Text.text = cooldown.ToString();
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill3Image, 0.1f);
        GameManager.Instance.ChangeTurnDelayed();
    }
}
