using TMPro;
using UnityEngine;

public class WalletView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _name = "Money: ";

    private Wallet _wallet;

    private void OnDisable()
    {
        _wallet.ValueChanged -= OnValueChanged;
    }

    public void SetWallet(Wallet wallet)
    {
        _wallet = wallet;
        _wallet.ValueChanged += OnValueChanged;
        OnValueChanged(_wallet.Value);
    }

    private void OnValueChanged(int value)
    {
        _text.text = $"{_name}{value.ToString()}";
    }
}