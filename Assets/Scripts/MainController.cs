using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField]
    IOController ioController;

    public 
    SceneController _sceneController;

    public
    PhysicsController _physicsController;

    [SerializeField]
    UIController _uiController;

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

        _uiController.Init(config.numUnitsToSpawn);
        _physicsController.onRemoveBlue.AddListener(() => _uiController.OnRemoveBlue());
        _physicsController.onRemoveRed.AddListener(() => _uiController.OnRemoveRed());
    }

    public void StartSim()
    {
        _physicsController.MoveStart();
    }
}
