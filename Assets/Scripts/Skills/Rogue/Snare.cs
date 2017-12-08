using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snare : Skill
{
    [HideInInspector]
    public int Immobilize;

    public SnareObject SnarePrefab;

    public override void Restart()
    {
        Cooldown = 10;
        ManaCost = 20;
        Damage = 50;
        Immobilize = 2;
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
            Immobilize += Lvl % 2;

            GameManager.Instance.ShowSkills();
        }
    }

    public override void Use(GenericEnemy enemy)
    {
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost) return;

        Player.Instance.currentManaPoints -= ManaCost;
        Player.Instance.UpdatePlayerMana();
        Player.Instance.animator.SetTrigger("attack"); 

        SnareObject snare = Instantiate(SnarePrefab, Player.Instance.transform.position, Quaternion.identity);
        snare.damage = Damage;
        snare.Immobilize = Immobilize;

        cooldown = Cooldown;
        GameManager.Instance.Skill1Text.text = cooldown.ToString();
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill1Image, 0.1f);
        GameManager.Instance.ChangeTurnDelayed();
    }
}
