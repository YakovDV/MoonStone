using UnityEngine.SceneManagement;
using Zenject;

public class SceneNavigator
{
    private readonly ZenjectSceneLoader _sceneLoader;

    public SceneNavigator(ZenjectSceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    public void LoadScene(string sceneName)
    {
        _sceneLoader.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    }

    public bool LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentSceneIndex < 0)
            return false;

        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
            return false;

        _sceneLoader.LoadScene(nextSceneIndex, LoadSceneMode.Single);

        return true;
    }
}
