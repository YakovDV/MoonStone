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
