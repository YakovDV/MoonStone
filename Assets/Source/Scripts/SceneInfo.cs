using System;
using UnityEngine;

[Serializable]
public class SceneInfo
{
    [SerializeField] private SceneID _id;
    [SerializeField] private string _sceneName;

    public SceneID ID => _id;
    public string SceneName => _sceneName;
}
