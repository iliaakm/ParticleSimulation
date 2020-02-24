using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IOController: MonoBehaviour
{
    const string _gameConfigURL = "GameConfig.json";        //конфиг игры

    public GameConfig ParseGameConfig()                     //читаем конфиг
    {
        string filepath = Path.Combine(Application.streamingAssetsPath, _gameConfigURL);
        string json = File.ReadAllText(filepath);
        GameConfig gameConfig = JsonUtility.FromJson<GameConfigRoot>(json).GameConfig;
        return gameConfig;
    }
}
