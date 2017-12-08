using UnityEngine;
using System.Collections;

public class Equipment {

    public Weapon weapon { get; set; }
    private Armor armor { get; set; }

    public Equipment(Weapon weapon, Armor armor)
    {
        this.weapon = weapon;
        this.armor = armor;
    }

    public int GetBonusDamage()
    {
        int bonusDamage = 0;

        bonusDamage += weapon.damage;

        return bonusDamage;
    }

    public int GetBonusArmor()
    {
        int bonusArmor = 0;

        bonusArmor += weapon.armor;
        bonusArmor += armor.armor;

        return bonusArmor;
    }
}
