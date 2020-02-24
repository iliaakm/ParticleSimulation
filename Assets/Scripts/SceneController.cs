using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    Transform _bg;
    [SerializeField]
    Camera _camera;
    [SerializeField]
    Transform _gameArea;
    [SerializeField]
    GameObject _unitPref;

    List<Enemy> _units = new List<Enemy>();
    Vector2 _areaSize;

    public void SetBGScale(int x, int y)
    {
        _bg.localScale = new Vector3(x, y, 1);
        _areaSize = new Vector2(x, y);
    }

    public void SetCameraSize(float width)
    {
        _camera.orthographicSize = width / 2;
    }

    public void Spawn(GameConfig config)
    {
        int count = config.numUnitsToSpawn;
        float delay = config.unitSpawnDelay;
        Vector2 radiusMinMax = new Vector2(config.unitSpawnMinRadius, config.unitSpawnMaxRadius);
        Vector2 speedMinMax = new Vector2(config.unitSpawnMinSpeed, config.unitSpawnMaxSpeed);

        StartCoroutine(SpawnRoutine(count, delay, radiusMinMax, speedMinMax));
    }

    IEnumerator SpawnRoutine(int count, float delay, Vector2 radius, Vector2 speed)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < count; i++)
        {
            Enemy blueOne = InstatiateUnit(count, radius, speed, Side.Blue);            //спавним синих
            blueOne.GetComponent<SpriteRenderer>().color = Color.blue;
            blueOne.name = "Blue_" + i;
            _units.Add(blueOne);

            Enemy redOne = InstatiateUnit(count, radius, speed, Side.Red);            //спавним красных
            redOne.GetComponent<SpriteRenderer>().color = Color.red;
            redOne.name = "Red_" + i;
            _units.Add(redOne);

            yield return new WaitForFixedUpdate();
        }

        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].MoveStart();
        }
        print("spawn done");
    }

    Vector2 GetRandomPos(float radius)
    {
        return GetRandomPos((int)_areaSize.x, (int)_areaSize.y, radius);
    }

    Vector2 GetRandomPos(int width, int height, float radius)
    {
        bool check = true;
        Vector2 pos = Vector2.zero;

        while (check)
        {
            check = false;
            float X = Random.Range(0, width) - width / 2;
            float Y = Random.Range(0, height) - height / 2;

            pos = new Vector2(X, Y);
            for (int i = 0; i < _units.Count; i++)
            {
                if (Vector2.Distance(_units[i].pos, pos) < (radius + _units[i]._radius) / 2)
                {
                    check = true;
                    break;
                }
            }
        }

        return pos;
    }

    Enemy InstatiateUnit(int count, Vector2 radius, Vector2 speed, Side side)
    {
        GameObject unit = Instantiate(_unitPref, _gameArea, false);
        unit.GetComponent<Enemy>()._speed = Random.Range(speed.x, speed.y);
        float _radius = Random.Range(radius.x, radius.y);
        unit.GetComponent<Enemy>()._radius = _radius;
        unit.transform.localPosition = GetRandomPos(_radius);
        unit.transform.rotation = Quaternion.identity;
        unit.transform.localScale = Vector3.one * unit.GetComponent<Enemy>()._radius;
        unit.GetComponent<Enemy>()._side = side;
        unit.GetComponent<Enemy>()._areaSize = _areaSize;

        return unit.GetComponent<Enemy>();
    }
}
