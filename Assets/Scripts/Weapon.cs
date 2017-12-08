using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Weapon : MonoBehaviour {

    public enum Types { Melee, Ranged };
    public enum Name { OneHandedSword, TwoHandedSword, Bow, Dagger, Rod, Staff, Hand};

    public string WeponName;
    public Sprite image;
    public int damage;
    public int armor;
    public Types type;
    public bool equiped;

    private int Lvl = 1;


    public int SellCost()
    {
        return (Lvl - 1) * (Lvl - 1) * 70 + 100;
    }


    public int UpgradeCost()
    {
        return Lvl * Lvl * 100;
    }

    public void UpgradeWeapon()
    {
        if (Player.Instance.gold < UpgradeCost()) return;

        Player.Instance.gold -= UpgradeCost();

        Lvl++;

        damage += 10;
        Player.Instance.damage += 10;

        if (armor != 0) armor += 2;
    }
}
