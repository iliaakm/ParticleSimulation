using System;

[Serializable]
public class GameConfig
{
    public int gameAreaWidth;               //ширина зоны
    public int gameAreaHeight;              //высота зоны
    public int numUnitsToSpawn;             //число юнитов для спавна
    public float unitSpawnDelay;            //задержка между спавнами (в секундах)
    public float unitSpawnMinRadius;        //мин радиус юнита
    public float unitSpawnMaxRadius;        //макс радиус юнита
    public float unitSpawnMinSpeed;         //мин скорость юнита
    public float unitSpawnMaxSpeed;         //макс скорость юнита
    public float unitDestroyRadius;         //радиус уничтожения
}

public class GameConfigRoot
{
    public GameConfig GameConfig;
}
