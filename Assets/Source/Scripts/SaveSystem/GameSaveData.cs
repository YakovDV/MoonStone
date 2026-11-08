using System;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    [SerializeField] private int _walletValue;
    [SerializeField] private int _totalProgress;
    [SerializeField] private UpgradeData _upgradeData;

    public int WalletValue => _walletValue;
    public int TotalProgress => _totalProgress;
    public UpgradeData UpgradeData => _upgradeData;

    public GameSaveData(int walletValue,int totalProgress, UpgradeData upgradeData)
    {
        _walletValue = walletValue;
        _upgradeData = upgradeData;
        _totalProgress = totalProgress;
    }
}