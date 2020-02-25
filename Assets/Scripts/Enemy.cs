using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side            
{
    None, Blue, Red
}

public class Enemy : MonoBehaviour
{
    public float Radius { get; private set; }
    public float Speed { get; private set; }
    public Side Side { get; private set; }
    public Vector2 CurrentCell { get; private set; }
    public Vector2 Pos { get; private set; }
    public Vector2 Target { get => _target; set => _target = value; }

    Vector2 _cellSize;
    Vector2 _areaSize;               //размер зоны
    int _axis;                       //текущая ось
    bool _canMove;                   //можем двигаться
    private Vector2 _target;

    public void Init(float radius, float speed, Side side, Vector2 pos, Vector2 areaSize, Vector2 cellSize)
    {
        Speed = speed;
        Side = side;
        SetPos(pos);
        SetRadius(radius);
        _areaSize = areaSize;
        _cellSize = cellSize;
    }

    public void StartMovement(bool redirect = true)         //начать(продолжить) движение
    {
        _canMove = true;
        if (redirect)
            ReDirection();
    }

    public void ReDirection()                               //задать новое направление    
    {
        Target = Vector2.zero;
        if (_axis == 0)
            _axis = RandomSign();        //-1 - X, 1 - Y
        else
            _axis *= -1;

        if (_axis < 0)
        {

            float x = RandomSign() * _areaSize.x / 2;
            float y = Random.Range(0, _areaSize.y) - _areaSize.y / 2;
            Target = new Vector2(x, y);
        }
        else
        {
            float y = RandomSign() * _areaSize.y / 2;
            float x = Random.Range(0, _areaSize.x) - _areaSize.x / 2;
            Target = new Vector2(x, y);
        }
    }

    private void Move()                                     //обсчет движения
    {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, Target, Speed * Time.fixedDeltaTime);
        Pos = transform.localPosition + (Vector3)_areaSize / 2;
        CurrentCell = new Vector2(Mathf.RoundToInt(Pos.x / _cellSize.x), Mathf.RoundToInt(Pos.y / _cellSize.y));

        Pos = transform.localPosition;
        if (Pos == Target)
            ReDirection();
    }

    private void FixedUpdate()
    {
        if (_canMove) Move();
    }

    int RandomSign()                                        //-1:1
    {
        return Random.value < 0.5f ? 1 : -1;
    }

    public void SetRadius(float radius)                     //задать радиус
    {
        Radius = radius;
        transform.localScale = Vector3.one * radius;
    }

    public void SetPos(Vector2 pos)                         //задать позицию
    {
        transform.localPosition = pos;
        Pos = pos;
    }
}