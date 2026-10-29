using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StartGamePanel : UIPanel
{
    [SerializeField] private TextMeshProUGUI _orderNumber;
    [SerializeField] private string _orderText;

    [SerializeField] private TextMeshProUGUI _orderGoal;
    [SerializeField] private string _orderGoalText;

    [SerializeField] private Button _startButton;

    private OrderConfig _config;

    public event Action StartReqested;

    private void OnEnable()
    {
        _startButton?.onClick.AddListener(RequestStart);
    }

    private void Start()
    {
        Initialize();
    }

    private void OnDisable()
    {
        _startButton?.onClick.RemoveListener(RequestStart);
    }

    [Inject]
    private void Construct(OrderConfig config)
    {
        _config = config;
    }

    private void Initialize()
    {
        _orderNumber.text = _orderText + _config.Name;
        _orderGoal.text = _orderGoalText + _config.TargetValue.ToString();
    }

    private void RequestStart()
    {
        StartReqested?.Invoke();
    }
}