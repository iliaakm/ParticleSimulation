using System;

[Serializable]
public class GameConfig
{
    public int gameAreaWidth;
    public int gameAreaHeight;
    public int numUnitsToSpawn;
    public float unitSpawnDelay;
    public float unitSpawnMinRadius;
    public float unitSpawnMaxRadius;
    public float unitSpawnMinSpeed;
    public float unitSpawnMaxSpeed;
    public float unitDestroyRadius;
}

public class GameConfigRoot
{
    public GameConfig GameConfig;
}
