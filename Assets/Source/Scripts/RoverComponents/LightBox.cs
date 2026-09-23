using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class LightBox : MonoBehaviour
{
    [SerializeField] private Material _offMat;
    [SerializeField] private Material _onMat;

    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void TurnOn()
    {
        if (_renderer.material == _onMat)
            return;

        _renderer.material = _onMat;
    }

    public void TurnOff()
    {
        if (_renderer.material == _offMat)
            return;

        _renderer.material = _offMat;
    }
}