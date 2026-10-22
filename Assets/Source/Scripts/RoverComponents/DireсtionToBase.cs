using UnityEngine;

public class DireсtionToBase : MonoBehaviour
{
    [SerializeField] private ResourceStorage _resourceStorage;
    [SerializeField] private Transform _targetPoint;

    [SerializeField] private CanvasGroup _arrowCanvas;

    private void OnEnable()
    {
        _resourceStorage.MassChanged += OnMassChanged;
        _arrowCanvas.alpha = 0;
    }

    private void OnDisable()
    {
        _resourceStorage.MassChanged -= OnMassChanged;
    }

    private void Update()
    {
        if (_arrowCanvas.alpha == 0)
            return;

        Vector3 direction =  _targetPoint.position - _resourceStorage.transform.position;

        _arrowCanvas.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    public void Initialize(ResourceStorage resourceStorage, Transform targetPoint)
    {
        _resourceStorage = resourceStorage;
        _targetPoint = targetPoint;

        _arrowCanvas.alpha = 0;
    }

    private void OnMassChanged(float mass)
    {
        if (Mathf.Approximately(mass, _resourceStorage.MaxCapacity))
        {
            if (_arrowCanvas.alpha == 1)
                return;

            _arrowCanvas.alpha = 1;
        }
        else if (Mathf.Approximately(mass, 0f))
        {
            if (_arrowCanvas.alpha == 0)
                return;

            _arrowCanvas.alpha = 0;
        }
    }
}
