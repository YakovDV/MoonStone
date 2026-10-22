using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanel : MonoBehaviour
{
    [SerializeField] private ScreenID _id;

    private CanvasGroup _canvasGroup;

    public ScreenID ID => _id;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Open()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 1.0f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}