using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

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
    [Inject]
    IOController _ioController;
    [Inject]
    SceneController _sceneController;
    [Inject]
    PhysicsController _physicsController;
    [Inject]
    UIController _uiController;

    public State _State
    {
        get { return state; }
        set
        {
            state = value;
            OnStateChange();
        }
    }

    State state;
    float _timeStart;

    [HideInInspector]
    public GameConfig _gameConfig;

    void Awake()
    {
        _gameConfig = _ioController.ParseGameConfig();
        Init(_gameConfig);

        _sceneController.onReady += SceneReady;
        _physicsController.onRemoveBlue += _uiController.OnRemoveBlue;
        _physicsController.onRemoveRed += _uiController.OnRemoveRed;
        _physicsController.onOver += Over;

        _State = State.Idle;
        Spawn(_gameConfig);
    }

    void Init(GameConfig config)                //инициализация
    {
        _sceneController.SetBGScale(config.gameAreaWidth, config.gameAreaHeight);

        _uiController.Init(config.numUnitsToSpawn);
    }

    public void StartSim()                      //начать симуляцию
    {
        if (_State == State.Ready)
        {
            _physicsController.MoveStart();
            _uiController.HidePlayBtn();
            _timeStart = Time.time;
            _State = State.Playing;
        }
    }

    void Spawn(GameConfig gameConfig)           //спаун юнитов
    {
        if (_State == State.Idle)
        {
            _State = State.Loading;
            _sceneController.Spawn(gameConfig);            
        }
    }               

    public void SceneReady()                    //готовность к запуску, юниты загружены
    {
        _uiController.ShowPlayBtn();
        _physicsController.OnRemoveBlue();
        _physicsController.OnRemoveRed();

        _State = State.Ready;
    }

    public void Over(Side side)                 //конец игры, показать победителя
    {
        float time = Time.time - _timeStart;
        _uiController.ShowGameOver(time, side);
        _State = State.Over;
    }

    public void Save()                          //сохранить
    {
        _ioController.Save();
    }

    public void Load()                          //загрузить
    {
        _State = State.Loading;
        SaveConfigList saves = _ioController.Load();
        _sceneController.Load(saves, _gameConfig.unitSpawnDelay);
    }                       

    public void Reload()                        //начать заново
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnStateChange()                        //действия при смене состояния
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
