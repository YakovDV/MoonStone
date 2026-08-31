using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    private Controls _controls;

    public Vector2 Move => _controls.Player.Move.ReadValue<Vector2>();
    public bool IsPausePressed => _controls.System.Pause.WasPressedThisFrame();

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        EnableGameplayInput();
        _controls.System.Enable();
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