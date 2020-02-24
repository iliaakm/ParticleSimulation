using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsController : MonoBehaviour
{
    [SerializeField]
    SceneController _sceneController;

    [HideInInspector]
    public UnityEvent onRemoveBlue;
    [HideInInspector]
    public UnityEvent onRemoveRed;

    float _fixeTimeDefault;

    bool handlePhysics = true;

    private void FixedUpdate()
    {
        if (handlePhysics) CalcCollisions();
    }

    private void CalcCollisions()
    {
        for (int i = 0; i < _sceneController._units.Count / 2; i++)
        {
            Enemy currentEnemy = _sceneController._units[i];
            int cellX = (int)currentEnemy._currentCell.x;
            int cellY = (int)currentEnemy._currentCell.y;

            List<Enemy> nearby = new List<Enemy>();
            nearby.AddRange(_sceneController._units.Where(x => x._currentCell.x == cellX && x != currentEnemy));
            if (cellX != 0) nearby.AddRange(_sceneController._units.Where(x => x._currentCell.x == cellX - 1));
            if (cellX != _sceneController._areaSize.x) nearby.AddRange(_sceneController._units.Where(x => x._currentCell.x == cellX + 1));
            nearby.AddRange(_sceneController._units.Where(x => x._currentCell.y == cellY && x != currentEnemy));
            if (cellY != 0) nearby.AddRange(_sceneController._units.Where(x => x._currentCell.x == cellY - 1));
            if (cellY != _sceneController._areaSize.y) nearby.AddRange(_sceneController._units.Where(x => x._currentCell.x == cellY + 1));

            nearby.RemoveAll(x => x == currentEnemy);
            nearby = nearby.Distinct().ToList();

            for (int y = 0; y < nearby.Count; y++)
            {
                Enemy oppositeEnemy = nearby[y]; 

                //if (currentEnemy == oppositeEnemy)
                //    print("pizdos");

                float radiusSum = (currentEnemy._radius + oppositeEnemy._radius) / 2;
                float distance = Vector2.Distance(currentEnemy.transform.localPosition, oppositeEnemy.transform.localPosition);
                if (distance <= radiusSum)
                {
                    if (currentEnemy._side == oppositeEnemy._side)
                    {
                        currentEnemy.MoveStart();
                        oppositeEnemy.MoveStart();
                        //Debug.Log("faced " + oppositeEnemy.name + " " + currentEnemy.name, oppositeEnemy.gameObject);
                    }
                    else
                    {
                        float radius = oppositeEnemy._radius * distance / oppositeEnemy._speed;
                        if (radius < _sceneController.minRadius)
                        {                          
                            //print("remove " + oppositeEnemy.name);
                            _sceneController._units.Remove(oppositeEnemy);
                            if (oppositeEnemy._side == Side.Blue) onRemoveBlue.Invoke();
                            if (oppositeEnemy._side == Side.Red) onRemoveRed.Invoke();

                            nearby.Remove(oppositeEnemy);
                            DestroyImmediate(oppositeEnemy.gameObject);
                        }
                        else
                            oppositeEnemy.SetRadius(radius);

                        radius = currentEnemy._radius * distance / currentEnemy._speed;
                        if (radius < _sceneController.minRadius)
                        {
                            //print("remove " + currentEnemy.name);
                            _sceneController._units.Remove(currentEnemy);
                            if (currentEnemy._side == Side.Blue) onRemoveBlue.Invoke();
                            if (currentEnemy._side == Side.Red) onRemoveRed.Invoke();
                            DestroyImmediate(currentEnemy.gameObject);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void MoveStart()
    {
        for (int i = 0; i < _sceneController._units.Count; i++)
        {
            _sceneController._units[i].MoveStart();
        }
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }
}
