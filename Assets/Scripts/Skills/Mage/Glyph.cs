using UnityEngine;

public class Glyph : Skill
{
    [HideInInspector]
    public int Armor;
    [HideInInspector]
    public Vector2 GlyphPosition;

    public GlyphObject glyph;

    public override void Restart()
    {
        Cooldown = 8;
        ManaCost = 25;
        Damage = 0;
        Armor = 30;
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
            Armor += 5;

            GameManager.Instance.ShowSkills();
        }
    }

    public override void Use(GenericEnemy enemy)
    {
        if (cooldown != 0 || Player.Instance.currentManaPoints < ManaCost) return;

        Player.Instance.currentManaPoints -= ManaCost;
        Player.Instance.UpdatePlayerMana();

        GlyphPosition = Player.Instance.transform.position;
        Instantiate(glyph, Player.Instance.transform.position, Quaternion.identity);

        cooldown = Cooldown;
        GameManager.Instance.Skill1Text.text = cooldown.ToString();
        GameManager.Instance.ChangeAlphaOfImage(GameManager.Instance.Skill1Image, 0.1f);
        GameManager.Instance.ChangeTurnDelayed();
    }
}
