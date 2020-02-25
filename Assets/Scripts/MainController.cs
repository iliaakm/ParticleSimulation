using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum State
{
    Idle,
    Loading,
    Ready,
    Playing,
    Over
}

public class MainController : MonoBehaviour
{
    public IOController _ioController;

    public SceneController _sceneController;

    public PhysicsController _physicsController;

    public UIController _uiController;

    State state;

    public State _State
    {
        get { return state; }
        set
        {
            state = value;
            OnStateChange();
        }
    }

    float _timeStart;

    void Awake()
    {
        GameConfig _gameConfig = _ioController.ParseGameConfig();
        Init(_gameConfig);

        _sceneController.onReady += SceneReady;
        _physicsController.onRemoveBlue += _uiController.OnRemoveBlue;
        _physicsController.onRemoveRed += _uiController.OnRemoveRed;
        _physicsController.onOver += Over;

        _State = State.Idle;
        Spawn(_gameConfig);
    }

    void Init(GameConfig config)
    {
        _sceneController.SetBGScale(config.gameAreaWidth, config.gameAreaHeight);
        _sceneController.SetCameraSize(config.gameAreaWidth);

        _uiController.Init(config.numUnitsToSpawn);
    }

    public void StartSim()
    {
        if (_State == State.Ready)
        {
            _physicsController.MoveStart();
            _uiController.HidePlayBtn();
            _timeStart = Time.time;
            _State = State.Playing;
        }
    }

    void Spawn(GameConfig gameConfig)
    {
        if (_State == State.Idle)
        {
            _State = State.Loading;
            _sceneController.Spawn(gameConfig);            
        }
    }

    public void SceneReady()
    {
        _uiController.ShowPlayBtn();
        _State = State.Ready;
    }

    public void Over(Side side)
    {
        float time = Time.time - _timeStart;
        _uiController.ShowGameOver(time, side);
        _State = State.Over;
    }

    public void Save()
    {
        _ioController.Save();
    }

    public void Load()
    {
        _State = State.Loading;
        SaveConfigList saves = _ioController.Load();
        _sceneController.Load(saves);
    }

    public void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnStateChange()
    {
        switch(_State)
        {
            case State.Loading:
                {
                    _uiController.HidePlayBtn();
                    break;
                }

            case State.Ready:
                {
                    _uiController.ShowPlayBtn();
                    break;
                }
        }
    }
}
