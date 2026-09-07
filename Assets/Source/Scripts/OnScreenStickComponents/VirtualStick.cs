using UnityEngine;
using UnityEngine.InputSystem;

public class VirtualStick : MonoBehaviour
{
    [SerializeField] private float _radius = 100f;
    [SerializeField] private RectTransform _activeZone;

    public Vector2 Value { get; private set; }
    public Vector2 Center { get; private set; }
    public Vector2 CurrentPosition { get; private set; }
    public bool IsActive { get; private set; }

    void Update()
    {
        var pointer = Pointer.current;

        if (IsActive == false && pointer.press.wasPressedThisFrame)
        {
            Center = pointer.position.ReadValue();

            if (RectTransformUtility.RectangleContainsScreenPoint(_activeZone, Center) == false)
                return;
            
            IsActive = true;
        }

        if (IsActive)
        {
            if (pointer.press.isPressed)
            {
                CurrentPosition = pointer.position.ReadValue();
            }
            else
            {
                Release();
            }
        }

        Value = IsActive ? Vector2.ClampMagnitude((CurrentPosition - Center) / _radius, 1f) : Vector2.zero;
    }

    private void Release()
    {
        IsActive = false;
        Center = Vector2.zero;
        CurrentPosition = Vector2.zero;
    }
}
