using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData", menuName = "Moon Stone/Resource data", order = 51)]
public class ResourceConfig : ScriptableObject
{
    [SerializeField] private string _id;

    [SerializeField] private int _value;
    [SerializeField] private float _mass;
    [SerializeField] private int _tier;

    public int Value => _value;
    public float Mass => _mass;
    public int Tier => _tier;
}