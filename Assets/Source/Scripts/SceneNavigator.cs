using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SceneNavigator
{
    private readonly ZenjectSceneLoader _sceneLoader;
    private readonly SceneCatalog _sceneCatalog;

    public SceneNavigator(ZenjectSceneLoader sceneLoader, SceneCatalog sceneCatalog)
    {
        _sceneLoader = sceneLoader;
        _sceneCatalog = sceneCatalog;
    }

    public AsyncOperation LoadScene(SceneID id)
    {
        if (_sceneCatalog.TryGetSceneName(id, out string sceneName) == false)
        {
            Debug.LogError($"Scene not found in catalog: {id}");
            return null;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName) == false)
        {
            Debug.LogError($"Scene not included in build: {sceneName}");
            return null;
        }

        return _sceneLoader.LoadSceneAsync(sceneName, LoadSceneMode.Single);
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