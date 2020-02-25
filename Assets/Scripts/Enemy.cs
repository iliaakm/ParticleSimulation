﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Side
{
    None, Blue, Red
}

public class Enemy : MonoBehaviour
{
    public Side _side;
    public float _radius;
    public float _speed;
    public Vector2 pos;
    public Vector2 _areaSize;
    public Vector2 _currentCell;
    public Vector2 _cellSize;

    Vector3 _target;
    int _axis;
    bool canMove;

    public void StartMovement()
    {
        canMove = true;
        ReDirection();
    }

    public void ReDirection()
    {
        _target = Vector2.zero;
        if (_axis == 0)
            _axis = RandomSign();        //-1 - X, 1 - Y
        else
            _axis *= -1;

        if(_axis < 0 )
        {
            _target.x = RandomSign() * _areaSize.x / 2;
            _target.y = Random.Range(0, _areaSize.y) - _areaSize.y / 2;
        }
        else
        {
            _target.y = RandomSign() * _areaSize.y / 2;
            _target.x = Random.Range(0, _areaSize.x) - _areaSize.x / 2;
        }
        _target.z = transform.localPosition.z;
    }

    private void Move()
    {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, _target, _speed * Time.fixedDeltaTime);
        Vector2 pos = transform.localPosition + (Vector3)_areaSize/2;
        _currentCell = new Vector2(Mathf.RoundToInt(pos.x / _cellSize.x), Mathf.RoundToInt(pos.y / _cellSize.y));

        if (transform.localPosition == _target)
            ReDirection();
    }

    private void FixedUpdate()
    {
        if (canMove) Move();
    }
    int RandomSign()
    {
        return Random.value < 0.5f ? 1 : -1;
    }

    public void SetRadius(float radius)
    {
        _radius = radius;
        transform.localScale = Vector3.one * radius;
    }
}