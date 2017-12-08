using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EndGameInfo {

    public int EnemiesKilled { get; set; }
    public int GoldGained { get; set; }
    public int LevelGained { get; set; }
    public int DungeonLevel { get; set; }


    public void Restart()
    {
        EnemiesKilled = 0;
        GoldGained = 0;
        LevelGained = 0;
        DungeonLevel = 0;
    }
}
