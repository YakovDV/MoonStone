using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DepositData", menuName = "Moon Stone/Deposit data", order = 51)]
public sealed class DepositConfig : ScriptableObject
{
    [SerializeField] private int _minIntegrity;
    [SerializeField] private int _maxIntegrity;
    [SerializeField] private ResourceDrop[] _drops;

    public int MinIntegrity => _minIntegrity;
    public int MaxIntegrity => _maxIntegrity;
    public IReadOnlyList<ResourceDrop> Drops => _drops;
}
