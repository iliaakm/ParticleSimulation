using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    MainController _mainController;
    [SerializeField]
    Image _progressRed, _progressBlue;
    [SerializeField]
    Button _btnStart;
    [SerializeField]
    Button _btnSave;
    [SerializeField]
    Button _btnLoad;
    [SerializeField]
    Slider _sliderSpeed;

    float _unitCount;

    public void Init(int count)
    {
        _unitCount = count;
        _progressBlue.color = Color.blue;
        _progressRed.color = Color.red;
        _btnStart.onClick.AddListener(() => _mainController.StartSim());
        _sliderSpeed.onValueChanged.AddListener((x) => _mainController._physicsController.SetTimeScale(_sliderSpeed.value));
    }

    public void OnRemoveRed()
    {
        int redCoint = _mainController._sceneController._units.Where(x => x._side == Side.Red).Count();
        _progressRed.fillAmount = redCoint / _unitCount * 0.5f;
    }

    public void OnRemoveBlue()
    {
        int blueCoint = _mainController._sceneController._units.Where(x => x._side == Side.Blue).Count();
        _progressBlue.fillAmount = blueCoint / _unitCount * 0.5f;
    }


}
