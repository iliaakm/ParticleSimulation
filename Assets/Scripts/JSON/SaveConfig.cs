using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveConfig
{
    public string id;
    public Side side;
    public float radius;
    public float speed;
    public Vector2 pos;
    public Vector2 target;
}

[Serializable]
public class SaveConfigList
{
    public SaveConfig[] saveConfigs;
}
