using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public class ButtonEffectPlayer : MonoBehaviour
{
    [SerializeField] private SFXID _sfxID;

    [Inject] private SFXPlayer _sfxPlayer;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    protected void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClicked);
    }

    protected void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }

    public void OnButtonClicked()
    {
        if (_button.IsActive() == false)
        {
            return;
        }

        _sfxPlayer.Play(_sfxID);
    }
}
