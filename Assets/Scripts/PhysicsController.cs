using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class PhysicsController : MonoBehaviour
{
    [Inject]
    SceneController _sceneController;
    [Inject]
    MainController _mainController;

    [HideInInspector]
    public UnityAction<int> onRemoveBlue;       //посчитать остаток синих
    [HideInInspector]
    public UnityAction<int> onRemoveRed;        //посчитать остаток красных
    [HideInInspector]
    public UnityAction<Side> onOver;            //завершить симуляцию

    private void Awake()
    {
        SetTimeScale(1f);
    }                     //дефолт timescale

    private void FixedUpdate()
    {
        if (_mainController._State == State.Playing) CalcCollisions();          
    }

    private void CalcCollisions()               //обсчет физики
    {
        for (int i = 0; i < _sceneController._units.Count; i++)         //пробегаемся по живым юнитам
        {
            Enemy currentEnemy = _sceneController._units[i];            
            int cellX = (int)currentEnemy.CurrentCell.x;               //ищем ячейку юнита, размер ячейки равен макс. диаметру
            int cellY = (int)currentEnemy.CurrentCell.y;

            List<Enemy> nearby = new List<Enemy>();
            nearby.AddRange(_sceneController._units.Where(x => x.CurrentCell.x == cellX && x != currentEnemy));
            if (cellX != 0) nearby.AddRange(_sceneController._units.Where(x => x.CurrentCell.x == cellX - 1));
            if (cellX != _sceneController._areaSize.x) nearby.AddRange(_sceneController._units.Where(x => x.CurrentCell.x == cellX + 1));
            nearby.AddRange(_sceneController._units.Where(x => x.CurrentCell.y == cellY && x != currentEnemy));
            if (cellY != 0) nearby.AddRange(_sceneController._units.Where(x => x.CurrentCell.x == cellY - 1));
            if (cellY != _sceneController._areaSize.y) nearby.AddRange(_sceneController._units.Where(x => x.CurrentCell.x == cellY + 1));

            nearby.RemoveAll(x => x == currentEnemy);           //проверка на самого себя
            nearby = nearby.Distinct().ToList();                //уничтожаем клонов

            for (int y = 0; y < nearby.Count; y++)
            {
                Enemy oppositeEnemy = nearby[y]; 

                float radiusSum = (currentEnemy.Radius + oppositeEnemy.Radius) / 2;
                float distance = Vector2.Distance(currentEnemy.transform.localPosition, oppositeEnemy.transform.localPosition);
                if (distance <= radiusSum)                              //если столкновение
                {
                    if (currentEnemy.Side == oppositeEnemy.Side)      //если одинаковые, то отталкиваемся
                    {
                        currentEnemy.ReDirection();
                        oppositeEnemy.ReDirection();
                        //Debug.Log("faced " + oppositeEnemy.name + " " + currentEnemy.name, oppositeEnemy.gameObject);
                    }
                    else                                 
                    {
                        float radius = oppositeEnemy.Radius * distance / oppositeEnemy.Speed;         //если разные, то уменьшаем
                        if (radius < _sceneController.minRadius)                                        //если меньше минимального, то уничтожаем            
                        {                          
                            //print("remove " + oppositeEnemy.name);
                            _sceneController._units.Remove(oppositeEnemy);
                            if (oppositeEnemy.Side == Side.Blue) OnRemoveBlue();
                            if (oppositeEnemy.Side == Side.Red) OnRemoveRed();

                            nearby.Remove(oppositeEnemy);
                            DestroyImmediate(oppositeEnemy.gameObject);
                        }
                        else
                            oppositeEnemy.SetRadius(radius);

                        radius = currentEnemy.Radius * distance / currentEnemy.Speed;
                        if (radius < _sceneController.minRadius)                        //если меньше минимального, то уничтожаем
                        {
                            //print("remove " + currentEnemy.name);
                            _sceneController._units.Remove(currentEnemy);
                            if (currentEnemy.Side == Side.Blue) OnRemoveBlue();
                            if (currentEnemy.Side == Side.Red) OnRemoveRed();

                            DestroyImmediate(currentEnemy.gameObject);
                            break;
                        }
                        else
                            currentEnemy.SetRadius(radius);
                    }
                }
            }
        }
    }               

    public void MoveStart()                     //запуск движения
    {
        for (int i = 0; i < _sceneController._units.Count; i++)
        {
            _sceneController._units[i].StartMovement();
        }
    }

    public void SetTimeScale(float value)       //управление скоростью симуляции
    {
        Time.timeScale = value;
    }

    public void OnRemoveRed()                   //по уничтожению красного
    {
        int redCoint = _sceneController._units.Where(x => x.Side == Side.Red).Count();
        if (redCoint == 0)
            onOver.Invoke(Side.Blue);
        onRemoveRed.Invoke(redCoint);
    }

    public void OnRemoveBlue()                  //по уничтожению синего
    {
        int blueCoint = _sceneController._units.Where(x => x.Side == Side.Blue).Count();
        if (blueCoint == 0)
            onOver.Invoke(Side.Red);
        onRemoveBlue.Invoke(blueCoint);
    }
}
