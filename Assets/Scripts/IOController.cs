using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IOController : MonoBehaviour
{
    public SceneController _sceneController;
    const string _gameConfigURL = "GameConfig.json";        //конфиг игры
    const string _savePattern = "EnemySave";                 //название сохранения

    public GameConfig ParseGameConfig()                     //читаем конфиг
    {
        string filepath = Path.Combine(Application.streamingAssetsPath, _gameConfigURL);
        string json = File.ReadAllText(filepath);
        GameConfig gameConfig = JsonUtility.FromJson<GameConfigRoot>(json).GameConfig;
        return gameConfig;
    }

    public void Save()
    {
        int unitN = _sceneController._units.Count;
        SaveConfigList saves = new SaveConfigList();
        saves.saveConfigs = new SaveConfig[unitN];

        for (int i = 0; i < unitN; i++)
        {
            Enemy enemy = _sceneController._units[i];
            saves.saveConfigs[i] = new SaveConfig();
            saves.saveConfigs[i].id = enemy.name;
            saves.saveConfigs[i].side = enemy._side;
            saves.saveConfigs[i].radius = enemy._radius;
            saves.saveConfigs[i].speed = enemy._speed;
            saves.saveConfigs[i].pos = enemy._pos;
            saves.saveConfigs[i].target = enemy._target;
        }

        string json = JsonUtility.ToJson(saves);
       
        PlayerPrefs.SetString(_savePattern, json);
        PlayerPrefs.Save();
        print("saved");
    }

    public SaveConfigList Load()
    {
        string json = PlayerPrefs.GetString(_savePattern);
        SaveConfigList saves = new SaveConfigList();
        saves = JsonUtility.FromJson<SaveConfigList>(json);
      

        return saves;
    }
}
