using UnityEngine;

public class VirtualStickView : MonoBehaviour
{
    [SerializeField] private RectTransform _root;
    [SerializeField] private VirtualStick _virtualStick;
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _handle;

    private void Update()
    {
        if (_virtualStick == null || _virtualStick.IsActive == false)
        {
            _background.gameObject.SetActive(false);
            _handle.gameObject.SetActive(false);

            return;
        }

        _background.gameObject.SetActive(true);
        _handle.gameObject.SetActive(true);

        _root.position = _virtualStick.Center;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, _virtualStick.Center, null, out Vector2 localCenter);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_background, _virtualStick.CurrentPosition, null, out Vector2 localCurrent);

        Vector2 offset = localCurrent - localCenter;

        float radius = _background.rect.width * 0.5f;

        _handle.localPosition = Vector2.ClampMagnitude(offset, radius);
    }
}
