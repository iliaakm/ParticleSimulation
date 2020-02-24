using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    Transform _bg;
    [SerializeField]
    Camera _camera;

    public void SetBGScale(int x, int y)
    {
        _bg.localScale = new Vector3(x, y, 1);
    }

    public void SetCameraSize(float width)
    {
        _camera.orthographicSize = width / 2;
    }
}
