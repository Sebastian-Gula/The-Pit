using UnityEngine;
using System.Collections;

[System.Serializable]
public class CharacterProperties : object
{
    public int HealthPoints;
    public int ManaPoints;
    public int StrengthPoints;
    public int AgilityPoints;
    public int IntelligencePoints;

    public static CharacterProperties operator +(CharacterProperties p1, CharacterProperties p2)
    {
        CharacterProperties p3 = new CharacterProperties();

        p3.HealthPoints = p1.HealthPoints + p2.HealthPoints;
        p3.ManaPoints = p1.ManaPoints + p2.ManaPoints;
        p3.StrengthPoints = p1.StrengthPoints + p2.StrengthPoints;
        p3.AgilityPoints = p1.AgilityPoints + p2.AgilityPoints;
        p3.IntelligencePoints = p1.IntelligencePoints + p2.IntelligencePoints;

        return p3;
    }

    public void Restart()
    {
        HealthPoints = 0;
        ManaPoints = 0;
        StrengthPoints = 0;
        AgilityPoints = 0;
        IntelligencePoints = 0;
    }
}
