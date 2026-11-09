using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneNavigator
{
    private readonly ZenjectSceneLoader _sceneLoader;
    private readonly SceneCatalog _sceneCatalog;
    private readonly ScreenFader _fader;

    private bool _isTransitioning = false;

    public SceneNavigator(ZenjectSceneLoader sceneLoader, SceneCatalog sceneCatalog, ScreenFader fader)
    {
        _sceneLoader = sceneLoader;
        _sceneCatalog = sceneCatalog;
        _fader = fader;
    }

    public async void LoadScene(SceneID id)
    {
        if (_sceneCatalog.TryGetSceneName(id, out string sceneName) == false)
        {
            Debug.LogError($"Scene not found in catalog: {id}");
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName) == false)
        {
            Debug.LogError($"Scene not included in build: {sceneName}");
            return;
        }

        await TryBeginTransition(sceneName);
    }

    private async Task TryBeginTransition(string sceneName)
    {
        if (_isTransitioning == true)
            return;

        _isTransitioning = true;

        try
        {
            await _fader.FadeInAsync();

            AsyncOperation operation = _sceneLoader.LoadSceneAsync(sceneName, LoadSceneMode.Single);

            while (operation.isDone == false)
            {
                await Task.Yield();
            }

            await _fader.FadeOutAsync();
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
            await _fader.FadeOutAsync();
        }
        finally
        {
            _isTransitioning = false;
        }
    }
}


[CreateAssetMenu(menuName = "Moon Stone/Scene Catalog")]
public class SceneCatalog : ScriptableObject
{
    [SerializeField] private SceneInfo[] _scenes;

    public bool TryGetSceneName(SceneID id, out string sceneName)
    {
        foreach (var scene in _scenes)
        {
            if (scene.ID == id)
            {
                sceneName = scene.SceneName;
                return !string.IsNullOrEmpty(sceneName);
            }
        }

        sceneName = null;
        return false;
    }
}

[Serializable]
public class SceneInfo
{
    [SerializeField] private SceneID _id;
    [SerializeField] private string _sceneName;

    public SceneID ID => _id;
    public string SceneName => _sceneName;
}

public enum SceneID
{
    MainMenu = 0,
    Level01 = 1,
    Level02 = 2
}
