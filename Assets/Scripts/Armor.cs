using UnityEngine;

public class Armor : MonoBehaviour {

    [HideInInspector]
    public bool firStyleActive = true;

    public string ArmorName;
    public int armor;
    public Sprite image
    {
        get
        {
            if (firStyleActive)
                return firstStyle;
            else
                return secondStyle;
        }
    }

    public Sprite firstStyle;
    public Sprite secondStyle;

    private int Lvl = 1;


    public int GetLvl()
    {
        return Lvl;
    }


    public int UpgradeCost()
    {
        return Lvl * Lvl * 100;
    }

    public void UpgradeArmor()
    {
        if (Player.Instance.gold < UpgradeCost()) return;

        Player.Instance.gold -= UpgradeCost();

        Lvl++;

        armor += 10;
    }
}
