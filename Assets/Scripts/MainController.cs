using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField]
    IOController ioController;

    [SerializeField]
    SceneManager _sceneManager;

    // GameConfig _gameConfig;

    void Awake()
    {
        GameConfig _gameConfig = ioController.ParseGameConfig();
        Init(_gameConfig);
    }

    void Init(GameConfig config)
    {
        _sceneManager.SetBGScale(config.gameAreaWidth, config.gameAreaHeight);
        _sceneManager.SetCameraSize(config.gameAreaWidth);
    }
}
