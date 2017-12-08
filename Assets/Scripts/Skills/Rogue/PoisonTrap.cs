using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTrap : Skill
{
    [HideInInspector]
    public int VenomDamage;
    [HideInInspector]
    public int VenomTime;

    public PoisonTrapObject PoisonTrapPrefab;


    public override void Restart()
    {
        Cooldown = 8;
        ManaCost = 25;
        Damage = 50;
        VenomTime = 2;
        VenomDamage = 10;
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
            Damage += 20;
            VenomTime += Lvl % 2;
            VenomDamage += 10;

            GameManager.Instance.ShowSkills();
        }
    }

    public override void Use(GenericEnemy enemy)
    {
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost) return;

        Player.Instance.currentManaPoints -= ManaCost;
        Player.Instance.UpdatePlayerMana();

        PoisonTrapObject poisonTrap = Instantiate(PoisonTrapPrefab, Player.Instance.transform.position, Quaternion.identity);
        poisonTrap.damage = Damage;
        poisonTrap.venomDamage = VenomDamage;
        poisonTrap.venomTime = VenomTime;

        cooldown = Cooldown;
        GameManager.Instance.Skill1Text.text = cooldown.ToString();
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill1Image, 0.1f);
        GameManager.Instance.ChangeTurnDelayed();
    }
}
