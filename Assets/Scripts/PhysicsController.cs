using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsController : MonoBehaviour
{
    [SerializeField]
    SceneController _sceneController;

    bool handlePhysics = true;


    private void FixedUpdate()
    {
        if (handlePhysics) CalcCollisions();
    }

    private void CalcCollisions()
    {
        for (int i = 0; i < _sceneController._units.Count / 2; i++)
        {
            int cellX = (int)_sceneController._units[i]._currentCell.x;
            int cellY = (int)_sceneController._units[i]._currentCell.y;

            List<Enemy> nearby = new List<Enemy>();
            nearby.AddRange(_sceneController._units.Where(x => x._currentCell.x == cellX && x != _sceneController._units[i]));
            if (cellX != 0) nearby.AddRange(_sceneController._units.Where(x => x._currentCell.x == cellX - 1));
            if (cellX != _sceneController._areaSize.x) nearby.AddRange(_sceneController._units.Where(x => x._currentCell.x == cellX + 1));
            nearby.AddRange(_sceneController._units.Where(x => x._currentCell.y == cellY && x != _sceneController._units[i]));
            if (cellY != 0) nearby.AddRange(_sceneController._units.Where(x => x._currentCell.x == cellY - 1));
            if (cellY != _sceneController._areaSize.y) nearby.AddRange(_sceneController._units.Where(x => x._currentCell.x == cellY + 1));

            nearby.RemoveAll(x => x == _sceneController._units[i]);
            nearby.RemoveAll(x => x == x);

            for (int y = 0; y < nearby.Count; y++)
            {
                if (_sceneController._units[i] == nearby[y])
                    print("pizdos");

                float radiusSum = (_sceneController._units[i]._radius + nearby[y]._radius) / 2;
                float distance = Vector2.Distance(_sceneController._units[i].transform.localPosition, nearby[y].transform.localPosition);
                if (distance <= radiusSum)
                {
                    if (_sceneController._units[i]._side == nearby[y]._side)
                    {
                        _sceneController._units[i].MoveStart();
                        nearby[y].MoveStart();
                        Debug.Log("faced " + nearby[y].name + " " + _sceneController._units[i].name, nearby[y].gameObject);
                    }
                    else
                    {
                        float radius = nearby[y]._radius * distance / nearby[y]._speed;
                        if (radius < _sceneController.minRadius)
                        {
                            print("remove " + nearby[y].name);
                            _sceneController._units.Remove(nearby[y]);
                            DestroyImmediate(nearby[y].gameObject);
                        }
                        else
                            nearby[y].SetRadius(radius);

                        radius = _sceneController._units[i]._radius * distance / _sceneController._units[i]._speed;
                        if (radius < _sceneController.minRadius)
                        {
                            print("remove " + _sceneController._units[i].name);
                            _sceneController._units.Remove(_sceneController._units[i]);
                            DestroyImmediate(_sceneController._units[i].gameObject);
                            break;
                        }
                    }
                }
            }
        }
    }
}
