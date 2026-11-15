using UnityEngine;
using Zenject;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private LevelCard[] _levelCards;

    [Inject] private SceneNavigator _sceneNavigator;
    [Inject] private SaveService _saveService;

    private void Start()
    {
        RefreshCards();
    }

    private void OnEnable()
    {
        for (int i = 0; i < _levelCards.Length; i++)
        {
            _levelCards[i].StartLevelRequested += OnStartLevelRequested;
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _levelCards.Length; i++)
        {
            _levelCards[i].StartLevelRequested -= OnStartLevelRequested;
        }
    }

    private void RefreshCards()
    {
        int progress = _saveService.GetProgress();

        for (int i = 0; i < _levelCards.Length; i++)
        {
            bool available = progress >= _levelCards[i].GetLevelRequiredProgress();

            _levelCards[i].RefreshInfo(available, progress);
        }
    }

    private void OnStartLevelRequested(SceneID sceneID)
    {
        _sceneNavigator.LoadScene(sceneID);
    }
}