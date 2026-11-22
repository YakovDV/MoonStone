using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationTable", menuName = "Moon Stone/Localization Table")]
public sealed class LocalizationTable : ScriptableObject
{
    [SerializeField] private TextAsset _source;

    public string Data => _source != null ? _source.text : string.Empty;
}