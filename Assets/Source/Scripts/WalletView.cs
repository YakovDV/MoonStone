using TMPro;
using UnityEngine;
using Zenject;

public class WalletView : MonoBehaviour
{
    [Inject] private Wallet _wallet;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _name = "Money: ";

    private void OnEnable()
    {
        _wallet.ValueChanged += OnValueChanged;
        OnValueChanged(_wallet.Value);
    }

    private void OnDisable()
    {
        _wallet.ValueChanged -= OnValueChanged;
    }

    private void OnValueChanged(int value)
    {
        _text.text = $"{_name}{value.ToString()}";
    }
}