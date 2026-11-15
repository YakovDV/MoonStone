using UnityEngine;

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
