using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartGamePanel : UIPanel
{
    [SerializeField] private TextMeshProUGUI _orderNumber;
    [SerializeField] private string _orderText;

    [SerializeField] private TextMeshProUGUI _orderGoal;
    [SerializeField] private string _orderGoalText;

    [SerializeField] private Button _startButton;

    public event Action StartReqested;

    private void OnEnable()
    {
        _startButton?.onClick.AddListener(RequestStart);
    }

    private void OnDisable()
    {
        _startButton?.onClick.RemoveListener(RequestStart);
    }

    public void Initilize(OrderConfig config)
    {
        _orderNumber.text = _orderText + config.Name;
        _orderGoal.text = _orderGoalText + config.TargetValue.ToString();
    }

    private void RequestStart()
    {
        StartReqested?.Invoke();
    }
}