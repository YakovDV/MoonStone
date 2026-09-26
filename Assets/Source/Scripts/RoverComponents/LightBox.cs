using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class LightBox : MonoBehaviour
{
    [SerializeField] private Material _offMat;
    [SerializeField] private Material _onMat;

    private Renderer _renderer;

    private Renderer Renderer => _renderer != null ? _renderer : _renderer = GetComponent<Renderer>();

    public void TurnOn()
    {
        if (Renderer.sharedMaterial == _onMat)
            return;

        Renderer.sharedMaterial = _onMat;
    }

    public void TurnOff()
    {
        if (Renderer.sharedMaterial == _offMat)
            return;

        Renderer.sharedMaterial = _offMat;
    }
}