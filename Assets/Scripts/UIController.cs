using System;
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
    Button _btnReload;
    [SerializeField]
    Slider _sliderSpeed;
    [SerializeField]
    GameObject _overPopup;
    [SerializeField]
    Text _popupText;

    float _unitCount;

    public void Init(int count)
    {
        _unitCount = count;
        _progressBlue.color = Color.blue;
        _progressRed.color = Color.red;
        _btnStart.onClick.AddListener(() => _mainController.StartSim());
        _btnSave.onClick.AddListener(() => _mainController.ioController.Save());
        _btnLoad.onClick.AddListener(() => _mainController.ioController.Load());
        _btnReload.onClick.AddListener(() => _mainController.Reload());
        _sliderSpeed.onValueChanged.AddListener((x) => _mainController._physicsController.SetTimeScale(_sliderSpeed.value));
    }

    public void ShowPlayBtn()
    {
        _btnStart.interactable = true;
    }

    public void HidePlayBtn()
    {
        _btnStart.interactable = false;
    }


    public void OnRemoveRed(int count)
    {
        _progressRed.fillAmount = count / _unitCount * 0.5f;
    }

    public void OnRemoveBlue(int count)
    {
        _progressBlue.fillAmount = count / _unitCount * 0.5f;
    }

    public void ShowGameOver(float time, Side sideWinner)
    {
        string winner = "Дружба";
        if (sideWinner == Side.Red)
            winner = "Красный";
        if (sideWinner == Side.Blue)
            winner = "Синий";

        string msg = String.Format("Победитель: {0}", winner);
        msg += System.Environment.NewLine;
        msg += String.Format("Время симуляции: {0} мс", time);

        _overPopup.SetActive(true);
        _popupText.text = msg;
    }
}
