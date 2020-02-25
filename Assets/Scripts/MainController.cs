using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour
{
public
    IOController ioController;

    public 
    SceneController _sceneController;

    public
    PhysicsController _physicsController;

  public
    UIController _uiController;

    float _timeStart;

    void Awake()
    {
        GameConfig _gameConfig = ioController.ParseGameConfig();
        Init(_gameConfig);
        _sceneController.Spawn(_gameConfig);
        _sceneController.onReady += SceneReady;
        _physicsController.onRemoveBlue += _uiController.OnRemoveBlue;
        _physicsController.onRemoveRed += _uiController.OnRemoveRed;
        _physicsController.onOver += Over;
    }

    void Init(GameConfig config)
    {
        _sceneController.SetBGScale(config.gameAreaWidth, config.gameAreaHeight);
        _sceneController.SetCameraSize(config.gameAreaWidth);

        _uiController.Init(config.numUnitsToSpawn);
    }

    public void StartSim()
    {
        _physicsController.MoveStart();
        _uiController.HidePlayBtn();
        _timeStart = Time.time;
    }

    public void SceneReady()
    {
        _uiController.ShowPlayBtn();
    }

    public void Over(Side side)
    {
        float time = Time.time - _timeStart;
        _uiController.ShowGameOver(time, side);
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
