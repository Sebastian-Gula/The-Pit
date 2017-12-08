using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrutalAttack : Skill
{
    private void Start()
    {
        Cooldown = 8;
        ManaCost = 20;
        Damage = Player.Instance.damage + 50;
    }

    public override void Restart()
    {
        Cooldown = 8;
        ManaCost = 20;
        Damage = Player.Instance.damage + 50;
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
            Damage += 30;

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
            enemy.TakeDamage(Damage);
            Player.Instance.animator.SetTrigger("brutalAttack");
            SoundManager.Instance.PlaySingle(Player.Instance.sword);

            cooldown = Cooldown;
            GameManager.Instance.Skill2Text.text = cooldown.ToString();
            GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill2Image, 0.1f);
        }

        GameManager.Instance.ChangeTurnDelayed();
    }
}
