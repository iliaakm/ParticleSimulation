using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveConfig
{
    public string id;           //имя юнита
    public Side side;           //сторона
    public float radius;        //текущий радиус    
    public float speed;         //скорость
    public Vector2 pos;         //текущая позиция
    public Vector2 target;      //точка направления
}

[Serializable]
public class SaveConfigList
{
    public SaveConfig[] saveConfigs;
}
