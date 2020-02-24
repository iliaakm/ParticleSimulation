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
    public float radius;
    public float speed;
}
