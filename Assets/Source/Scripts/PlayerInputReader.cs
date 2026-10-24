using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private VirtualStick _virtualStick;

    public Vector2 Move => GetMove();
    public bool IsPausePressed => _controls.System.Pause.WasPressedThisFrame();

    private bool _isMoveEnabled;

    private Controls _controls;

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        EnableGameplayInput();
        _controls.System.Enable();

        _isMoveEnabled = true;
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
        _controls.System.Disable();

        _isMoveEnabled = false;
    }

    private void OnDestroy()
    {
        _controls.Dispose();
    }

    public Vector2 GetMove()
    {
        if(_isMoveEnabled == false)
            return Vector2.zero;

        if (_virtualStick != null)
        {
            if (_virtualStick.IsActive)
                return _virtualStick.Value;
        }

        return _controls.Player.Move.ReadValue<Vector2>();
    }

    public void EnableGameplayInput()
    {
        _controls.Player.Enable();
        _isMoveEnabled = true;
    }

    public void DisableGameplayInput()
    {
        _controls.Player.Disable();
        _isMoveEnabled = false;
    }
}