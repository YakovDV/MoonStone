using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _sourceA;
    [SerializeField] private AudioSource _sourceB;
    [SerializeField] private Music[] _music;
    [SerializeField] private MusicID _defaultMusicID;
    [SerializeField, Min(0f)] private float _fadeDuration = 1.5f;

    private AudioSource _current;
    private Coroutine _transition;
    private bool _isStopping;

    private void Awake()
    {
        _sourceA.playOnAwake = false;
        _sourceB.playOnAwake = false;

        _sourceA.loop = true;
        _sourceB.loop = true;

        _sourceA.volume = 0f;
        _sourceB.volume = 0f;

        _current = _sourceA;
    }

    private void Start()
    {
        Play(_defaultMusicID);
    }

    public void Play(MusicID musicID)
    {
        AudioClip clip = GetClip(musicID);

        if (clip == null)
        {
            Debug.LogWarning($"Music not found: {musicID}");
            return;
        }

        if (_current.clip == clip && _current.isPlaying)
        {
            if (_isStopping == false)
                return;

            CancelTransition();
            _isStopping = false;
            _transition = StartCoroutine(FadeIn(_current));
            return;
        }

        CancelTransition();
        _isStopping = false;

        AudioSource next = _current == _sourceA ? _sourceB : _sourceA;

        next.Stop();
        next.clip = clip;
        next.volume = 0f;
        next.Play();

        AudioSource previous = _current;
        _current = next;

        _transition = StartCoroutine(Crossfade(previous, next));
    }

    public void Stop()
    {
        CancelTransition();

        AudioSource inactive = _current == _sourceA ? _sourceB : _sourceA;

        inactive.Stop();
        inactive.volume = 0f;

        if (_current.isPlaying == false)
            return;

        _isStopping = true;
        _transition = StartCoroutine(FadeOut(_current));
    }

    private IEnumerator Crossfade(AudioSource previous, AudioSource next)
    {
        float startVolume = previous.volume;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / _fadeDuration);

            previous.volume = Mathf.Lerp(startVolume, 0f, t);
            next.volume = t;

            yield return null;
        }

        previous.Stop();
        previous.volume = 0f;
        next.volume = 1f;
        _transition = null;
    }

    private IEnumerator FadeIn(AudioSource source)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, 1f, Mathf.Clamp01(elapsed / _fadeDuration));

            yield return null;
        }

        source.volume = 1f;
        _transition = null;
    }

    private IEnumerator FadeOut(AudioSource source)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, Mathf.Clamp01(elapsed / _fadeDuration));

            yield return null;
        }

        source.Stop();
        source.volume = 0f;
        _isStopping = false;
        _transition = null;
    }

    private AudioClip GetClip(MusicID musicID)
    {
        foreach (var music in _music)
        {
            if (music.MusicID == musicID)
                return music.AudioClip;
        }

        return null;
    }

    private void CancelTransition()
    {
        if (_transition == null)
            return;

        StopCoroutine(_transition);
        _transition = null;
    }
}
