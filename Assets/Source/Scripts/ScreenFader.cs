using UnityEngine;
using System.Threading.Tasks;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField, Min(0f)] private float _fadeDuration = 0.5f;

    public Task FadeInAsync() => RunFadeAsync(1f);
    public Task FadeOutAsync() => RunFadeAsync(0f);

    private void Awake()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
    }

    private async Task RunFadeAsync(float targetAlpha)
    {
        float startAlpha = _canvasGroup.alpha;
        float elapsed = 0f;

        _canvasGroup.blocksRaycasts = true;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _fadeDuration);

            await Task.Yield();
        }

        _canvasGroup.alpha = targetAlpha;
        _canvasGroup.blocksRaycasts = false;
    }
}