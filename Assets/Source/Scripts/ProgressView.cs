using TMPro;
using UnityEngine;

public class ProgressView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private char _separator = '/';
    [SerializeField] private string _name;

    private float _goal;

    private Order _order;

    private void OnDisable()
    {
        _order.ProgressChanged -= OnProgressChanged;
    }

    public void SetOrder(Order order)
    {
        _order = order;
        _order.ProgressChanged += OnProgressChanged;

        _goal = _order.RequiredValue;

        OnProgressChanged(_order.Progress);
    }

    private void OnProgressChanged(float value)
    {
        _text.text = $"{_name}{value.ToString()}{_separator}{_goal}";
    }
}