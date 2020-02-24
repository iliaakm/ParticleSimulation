using System.Collections;
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

    Vector3 _target;
    int _axis;
    bool canMove;

    public void MoveStart()
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
        
        canMove = true;
    }

    private void Move()
    {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, _target, _speed * Time.fixedDeltaTime);
        if (transform.localPosition == _target)
            MoveStart();
    }

    private void FixedUpdate()
    {
        if (canMove) Move();
    }
    int RandomSign()
    {
        return Random.value < 0.5f ? 1 : -1;
    }
}
