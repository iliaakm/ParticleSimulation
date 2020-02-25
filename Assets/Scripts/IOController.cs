using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class IOController : MonoBehaviour
{
    [Inject]
    SceneController _sceneController;
    const string _gameConfigURL = "GameConfig.json";         //конфиг игры
    const string _savePattern = "EnemySave";                 //название сохранения

    public GameConfig ParseGameConfig()                     //читаем конфиг
    {
        string json = "";
        if (Application.platform == RuntimePlatform.Android)                    //т.к. json упакован в apk, читаем его отдельно
        {
            string filePath = "jar:file://" + Application.dataPath + "!/assets/" + _gameConfigURL;

            WWW reader = new WWW(filePath);
            while (!reader.isDone) { }

            json = reader.text;
        }
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, _gameConfigURL);
            json = File.ReadAllText(filePath);
        }

        GameConfig gameConfig = JsonUtility.FromJson<GameConfigRoot>(json).GameConfig;
        return gameConfig;
    }

    public void Save()                                       //сохраняем положение                   
    {
        int unitN = _sceneController._units.Count;
        SaveConfigList saves = new SaveConfigList();
        saves.saveConfigs = new SaveConfig[unitN];

        for (int i = 0; i < unitN; i++)
        {
            Enemy enemy = _sceneController._units[i];
            saves.saveConfigs[i] = new SaveConfig
            {
                id = enemy.name,
                side = enemy.Side,
                radius = enemy.Radius,
                speed = enemy.Speed,
                pos = enemy.Pos,
                target = enemy.Target
            };
        }

        string json = JsonUtility.ToJson(saves);

        PlayerPrefs.SetString(_savePattern, json);
        PlayerPrefs.Save();
        print("saved");
    }

    public SaveConfigList Load()                            //чтение из памяти и парсинг
    {
        string json = "";                                   
        try
        {
            json = PlayerPrefs.GetString(_savePattern);     //на случай отсутствия
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

        SaveConfigList saves = new SaveConfigList();
        saves = JsonUtility.FromJson<SaveConfigList>(json);
        return saves;
    }
}
