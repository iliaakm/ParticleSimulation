using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class SceneController : MonoBehaviour
{
    [Inject]
    MainController _mainController;          //главный контроллер                                   

    [SerializeField]
    Transform _bg;                          //фон
    [SerializeField]
    Camera _camera;                         //главная камера
    [SerializeField]
    Transform _gameArea;                    //игровая зона
    [SerializeField]
    GameObject _unitPref;                   //префаб юнита

    public List<Enemy> _units = new List<Enemy>();          //список активных юнитов
    public UnityAction onReady;                             //эвент по окончанию загрузки

    Vector2 _areaSize;                                      //размер зоны ()
    float _screenRatio;                                     //последнее соотношение сторон экрана    
    Coroutine cor;                                          //переменная корутин    

    public void SetBGScale(int x, int y)                                //установка игровой зоны
    {
        _bg.localScale = new Vector3(x, y, 1);
        _areaSize = new Vector2(x, y);
    }

    private void FixedUpdate()                                          //для экономии ресурсов, запускаем в FixedUpdate
    {
        ScreenRatioHndler();
    }

    void ScreenRatioHndler()                                            //проверка соотношения сторон экрана
    {
        float screenRatio = (float)Screen.height / (float)Screen.width;

        if (screenRatio == _screenRatio)
            return;

        if (screenRatio > 1)
            Camera.main.orthographicSize = screenRatio * _areaSize.x / 2;
        else
            _camera.orthographicSize = _areaSize.x / 2;
    }

    public void Spawn(GameConfig config)
    {
        if (cor != null)
            StopCoroutine(cor);
        cor = StartCoroutine(SpawnRoutine(config));
    }                            //запуск спауна

    IEnumerator SpawnRoutine(GameConfig config)                         //корутина спауна
    {
        int count = config.numUnitsToSpawn;
        float delay = config.unitSpawnDelay;
        Vector2 radiusMinMax = new Vector2(config.unitSpawnMinRadius, config.unitSpawnMaxRadius);
        Vector2 speedMinMax = new Vector2(config.unitSpawnMinSpeed, config.unitSpawnMaxSpeed);
        Vector2 cellSize = new Vector2(radiusMinMax.y, radiusMinMax.y);                                 //размер ячейки для обсчета физики

        for (int i = 0; i < count; i++)
        {
            string name = "Blue_" + i;
            Enemy blueOne = InstatiateUnitRandom(name, radiusMinMax, speedMinMax, cellSize, Side.Blue);            //спавним синих

            name = "Red_" + i;
            Enemy redOne = InstatiateUnitRandom(name, radiusMinMax, speedMinMax, cellSize, Side.Red);            //спавним красных

            yield return new WaitForSeconds(delay);
        }
        onReady.Invoke();
    }

    Vector2 GetRandomPos(float radius)                                  //рандомная позиция для радиуса
    {
        return GetRandomPos((int)_areaSize.x, (int)_areaSize.y, radius);
    }                                                              

    Vector2 GetRandomPos(int width, int height, float radius)           //рандомная позиция для радиуса в заданном прямоугольнике    
    {
        bool check = true;
        Vector2 pos = Vector2.zero;
        int counter = 0;

        while (check)
        {
            check = false;
            counter++;
            if (counter == 1000)                                        //по истечению 1000 неудачных попыток, аварийно выходим
            {
                Debug.LogError("No more space");
                return Vector2.zero;
            }

            float X = Random.Range(0, width) - width / 2;
            float Y = Random.Range(0, height) - height / 2;

            pos = new Vector2(X, Y);
            for (int i = 0; i < _units.Count; i++)
            {
                if (Vector2.Distance(_units[i].Pos, pos) < (radius + _units[i].Radius) / 2)
                {
                    check = true;
                    break;
                }
            }
        }

        return pos;
    }

    Enemy InstatiateUnitRandom(string name, Vector2 radius, Vector2 speed, Vector2 cellSize, Side side)         //спавн конкретного юнита с рандомными пар-ми
    {
        SaveConfig unitConfig = new SaveConfig();
        unitConfig.id = name;
        unitConfig.radius = Random.Range(radius.x, radius.y);
        unitConfig.speed = Random.Range(speed.x, speed.y);
        unitConfig.pos = GetRandomPos(unitConfig.radius);
        unitConfig.side = side;

        return InstatiateUnit(unitConfig, cellSize);
    }

    Enemy InstatiateUnit(SaveConfig unitConfig, Vector2 cellSize)                                                   //спавн конкретного юнита с конкретными пар-ми
    {
        GameObject unit = Instantiate(_unitPref, _gameArea, false);
        unit.name = unitConfig.id;
        unit.transform.rotation = Quaternion.identity;

        Enemy enemy = unit.GetComponent<Enemy>();
        enemy.Init(
            unitConfig.radius,
            unitConfig.speed,
            unitConfig.side,
            unitConfig.pos,
            _areaSize,
            cellSize);
            
        unit.GetComponent<SpriteRenderer>().color = (enemy.Side == Side.Blue) ? Color.blue : Color.red;
        _units.Add(enemy);

        return unit.GetComponent<Enemy>();
    }                                            

    public void Load(SaveConfigList saves, float delay)                                                             //запуск загрузки
    {
        if (cor != null)
            StopCoroutine(cor);
        cor = StartCoroutine(LoadRountine(saves, delay));
    }

    IEnumerator LoadRountine(SaveConfigList saves, float delay)                                                     //загрузка из памяти
    {
        for (int i = 0; i < _units.Count; i++)
            DestroyImmediate(_units[i].gameObject);

        _units.Clear();
        float radiusMinMax = _mainController.gameConfig.unitSpawnMaxRadius;
        Vector2 cellSize = new Vector2(radiusMinMax, radiusMinMax);

        for (int i = 0; i < saves.saveConfigs.Length; i++)
        {
            InstatiateUnit(saves.saveConfigs[i], cellSize);
            yield return new WaitForSeconds(delay);
        }

        onReady.Invoke();
    }
}
