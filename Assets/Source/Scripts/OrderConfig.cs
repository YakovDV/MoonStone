using UnityEngine;

[CreateAssetMenu]
public class OrderConfig : ScriptableObject
{
    public int RequiredProgress;
    public int TargetValue;
    public string Name;
    public string Description;
}
