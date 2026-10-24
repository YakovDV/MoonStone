using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Deposit))]
public class DepositIntegrityView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private string _textSeparator = " / ";

    [SerializeField] private float _disappearDelay = 2f;
    [SerializeField] private float _fadeDuration = 0.2f;

    private Deposit _deposit;

    private Coroutine _changeVisibilitySmooth;

    private void Awake()
    {
        _deposit = GetComponent<Deposit>();
        _text.alpha = 0f;
    }

    private void OnEnable()
    {
        _deposit.ValueChanged += OnValueChanged;
    }

    private void OnDisable()
    {
        _deposit.ValueChanged -= OnValueChanged;
    }

    private void OnValueChanged(int value)
    {
        _text.text = value.ToString() + _textSeparator + _deposit.BaseIntegrity;

        if (_changeVisibilitySmooth != null)
        {
            StopCoroutine(_changeVisibilitySmooth);
            _changeVisibilitySmooth = null;
        }

        _changeVisibilitySmooth = StartCoroutine(ShowAndHide());
    }

    private IEnumerator ChangeVisibilitySmooth(float duration, float startAlphaValue, float targetAlphaValue)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            t = Mathf.SmoothStep(0f, 1f, t);

            _text.alpha = Mathf.Lerp(startAlphaValue, targetAlphaValue, t);

            yield return null;
        }

        _text.alpha = targetAlphaValue;
    }

    private IEnumerator ShowAndHide()
    {
        yield return ChangeVisibilitySmooth(_fadeDuration, _text.alpha, 1f);

        yield return new WaitForSeconds(_disappearDelay);

        yield return ChangeVisibilitySmooth(_fadeDuration, _text.alpha, 0f);

        _changeVisibilitySmooth = null;
    }
}