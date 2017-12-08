using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Skill : MonoBehaviour
{
    public string Name;
    public Sprite Image;

    [HideInInspector]
    public int Cooldown;
    [HideInInspector]
    public int cooldown = 0;
    [HideInInspector]
    public int ManaCost;
    [HideInInspector]
    public int Damage;
    [HideInInspector]
    public int Lvl = 1;


    public void DecreaseCooldown(Image image, Text text)
    {
        if (cooldown > 0)
        {
            cooldown--;

            if (cooldown == 0)
            {
                GameManager.Instance.ChangeAlphaOfImage(image, 1f);
                text.text = "";
            }
            else
                text.text = cooldown.ToString();
        }
    }


    public void Load()
    {
        Restart();
        cooldown = 0;

        for (int i = 1; i < PlayerPrefs.GetInt(Name); i++)
            Upgrade();
    }


    public abstract void Restart();


    public void Save()
    {
        PlayerPrefs.SetInt(Name, Lvl);
    }


    public int UpgradeCost()
    {
        return Lvl * 1/2 + 1;
    }

    public abstract void Upgrade();

    public abstract void Use(GenericEnemy enemy);
}
