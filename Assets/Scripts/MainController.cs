using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField]
    IOController ioController;

    [SerializeField]
    SceneController _sceneController;

    // GameConfig _gameConfig;

    void Awake()
    {
        GameConfig _gameConfig = ioController.ParseGameConfig();
        Init(_gameConfig);
        _sceneController.Spawn(_gameConfig);
    }

    void Init(GameConfig config)
    {
        _sceneController.SetBGScale(config.gameAreaWidth, config.gameAreaHeight);
        _sceneController.SetCameraSize(config.gameAreaWidth);
    }
}
