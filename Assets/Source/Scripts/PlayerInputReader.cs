using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    [SerializeField] private VirtualStick _virtualStick;

    public Vector2 Move => GetMove();
    public bool IsPausePressed => _controls.System.Pause.WasPressedThisFrame();
    
    private Controls _controls;

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        EnableGameplayInput();
        _controls.System.Enable();
    }

    public Vector2 GetMove()
    {
        if (_virtualStick.IsActive)
            return _virtualStick.Value;

        return _controls.Player.Move.ReadValue<Vector2>();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
        _controls.System.Disable();
    }

    private void OnDestroy()
    {
        _controls.Dispose();
    }

    public void EnableGameplayInput()
    {
        _controls.Player.Enable();
    }

    public void DisableGameplayInput()
    {
        _controls.Player.Disable();
    }
}