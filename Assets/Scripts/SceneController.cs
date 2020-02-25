using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SceneController : MonoBehaviour
{
    public MainController _mainController;
    [SerializeField]
    Transform _bg;
    [SerializeField]
    Camera _camera;
    [SerializeField]
    Transform _gameArea;
    [SerializeField]
    GameObject _unitPref;

    public int cameraHeight;

    public List<Enemy> _units = new List<Enemy>();
    public Vector2 _areaSize;
    public float minRadius;
    public UnityAction onReady;

    Coroutine cor;

    public void SetBGScale(int x, int y)
    {
        _bg.localScale = new Vector3(x, y, 1);
        _areaSize = new Vector2(x, y);
    }

    private void FixedUpdate()
    {
        float aspect = (float)Screen.height / (float)Screen.width;
        if (aspect > 1)
            Camera.main.orthographicSize = aspect * _areaSize.x/2;
        else
            _camera.orthographicSize = _areaSize.x / 2;
    }

    public void Spawn(GameConfig config)
    {
        if (cor != null)
            StopCoroutine(cor);
        cor = StartCoroutine(SpawnRoutine(config));
    }

    IEnumerator SpawnRoutine(GameConfig config)
    {
        int count = config.numUnitsToSpawn;
        float delay = config.unitSpawnDelay;
        Vector2 radiusMinMax = new Vector2(config.unitSpawnMinRadius, config.unitSpawnMaxRadius);
        Vector2 speedMinMax = new Vector2(config.unitSpawnMinSpeed, config.unitSpawnMaxSpeed);
        Vector2 cellSize = new Vector2(radiusMinMax.y, radiusMinMax.y);
        minRadius = config.unitDestroyRadius;

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

    Vector2 GetRandomPos(float radius)
    {
        return GetRandomPos((int)_areaSize.x, (int)_areaSize.y, radius);
    }

    Vector2 GetRandomPos(int width, int height, float radius)
    {
        bool check = true;
        Vector2 pos = Vector2.zero;
        int counter = 0;

        while (check)
        {
            check = false;
            counter++;
            if (counter == 1000)
            {
                Debug.LogError("No more space");
                return Vector2.zero;
            }

            float X = Random.Range(0, width) - width / 2;
            float Y = Random.Range(0, height) - height / 2;

            pos = new Vector2(X, Y);
            for (int i = 0; i < _units.Count; i++)
            {
                if (Vector2.Distance(_units[i]._pos, pos) < (radius + _units[i]._radius) / 2)
                {
                    check = true;
                    break;
                }
            }
        }

        return pos;
    }

    Enemy InstatiateUnitRandom(string name, Vector2 radius, Vector2 speed, Vector2 cellSize, Side side)
    {
        SaveConfig unitConfig = new SaveConfig();
        unitConfig.id = name;
        unitConfig.radius = Random.Range(radius.x, radius.y);
        unitConfig.speed = Random.Range(speed.x, speed.y);
        unitConfig.pos = GetRandomPos(unitConfig.radius);
        unitConfig.side = side;

        return InstatiateUnit(unitConfig, cellSize);
    }

    Enemy InstatiateUnit(SaveConfig unitConfig, Vector2 cellSize)
    {
        GameObject unit = Instantiate(_unitPref, _gameArea, false);
        unit.name = unitConfig.id;
        unit.transform.rotation = Quaternion.identity;

        Enemy enemy = unit.GetComponent<Enemy>();
        enemy.SetPos(unitConfig.pos);
        enemy.SetRadius(unitConfig.radius);
        enemy._speed = unitConfig.speed;
        enemy._side = unitConfig.side;
        enemy._areaSize = _areaSize;
        enemy._cellSize = cellSize;
        unit.GetComponent<SpriteRenderer>().color = (enemy._side == Side.Blue) ? Color.blue : Color.red;
        _units.Add(enemy);

        return unit.GetComponent<Enemy>();
    }

    public void Load(SaveConfigList saves, float delay)
    {
        if (cor != null)
            StopCoroutine(cor);
        cor = StartCoroutine(LoadRountine(saves, delay));
    }

    IEnumerator LoadRountine(SaveConfigList saves, float delay)
    {
        for (int i = 0; i < _units.Count; i++)
            DestroyImmediate(_units[i].gameObject);

        _units.Clear();
        float radiusMinMax = _mainController._gameConfig.unitSpawnMaxRadius;
        Vector2 cellSize = new Vector2(radiusMinMax, radiusMinMax);

        for (int i = 0; i < saves.saveConfigs.Length; i++)
        {
            InstatiateUnit(saves.saveConfigs[i], cellSize);
            yield return new WaitForSeconds(delay);

        }

        onReady.Invoke();
    }
}
