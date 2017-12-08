using UnityEngine;

public class PoisonMissile : Skill
{
    [HideInInspector]
    public int VenomDamage;
    [HideInInspector]
    public int VenomTime;

    public Rigidbody2D poisonMissile;


    public override void Restart()
    {
        Cooldown = 8;
        ManaCost = 25;
        Damage = 0;
        VenomTime = 2;
        VenomDamage = 30;
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
            VenomTime += Lvl % 2;
            VenomDamage += 15;

            GameManager.Instance.ShowSkills();
        }
    }

    public override void Use(GenericEnemy enemy)
    {
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost || !Player.Instance.CanShoot(enemy.transform.position)) return;

        Player.Instance.currentManaPoints -= ManaCost;
        Player.Instance.UpdatePlayerMana();

        Player.Instance.animator.SetTrigger("attack");
        Player.Instance.Shoot(enemy.transform.position, poisonMissile, Player.Instance.damage, VenomDamage, VenomTime);

        cooldown = Cooldown;
        GameManager.Instance.Skill2Text.text = cooldown.ToString();
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill2Image, 0.1f);
        Player.Instance.BlockUpdate();
    }
}
