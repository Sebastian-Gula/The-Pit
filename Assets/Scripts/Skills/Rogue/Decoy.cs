using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : Skill {

    public DecoyObject decoy;

    
    private bool CheckArea()
    {
        int chance = 10;

        Vector2 playerPosition = Player.Instance.transform.position;
        int sx = (int)playerPosition.x - 1;
        int ex = (int)playerPosition.x + 1;
        int sy = (int)playerPosition.y - 1;
        int ey = (int)playerPosition.y + 1;

        for (int x = sx; x <= ex; x++)
        {
            for (int y = sy; y <= ey; y++)
            {
                chance += 10;

                if (playerPosition.Equals(new Vector2(x + 0.5f, y + 0.5f)))
                    continue;

                int rand = Random.Range(0, 100);

                if (rand < chance && BoardManager.obstacles[x][y] != 'X')
                {
                    Instantiate(decoy, new Vector2(x + 0.5f, y + 0.5f), Quaternion.identity);
                    DecoyObject.Instance.health = Player.Instance.currentHealthPoints;
                    DecoyObject.Instance.turns = 5;
                    return true;
                }
            }
        }

        return false;
    }


    public override void Restart()
    {
        Cooldown = 20;
        ManaCost = 50;
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
            ManaCost -= 2;

            GameManager.Instance.ShowSkills();
        }
    }

    public override void Use(GenericEnemy enemy)
    {
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost) return;

        if(CheckArea())
        {
            Player.Instance.currentManaPoints -= ManaCost;
            Player.Instance.UpdatePlayerMana();

            cooldown = Cooldown;
            GameManager.Instance.Skill3Text.text = cooldown.ToString();
            GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill3Image, 0.1f);
            GameManager.Instance.DecoyIsUp = true;
            GameManager.Instance.ChangeTurnDelayed();
        }
    }
}
