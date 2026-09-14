using System;

[Serializable]
public class DepositSpawnerLink
{
    public DepositConfig Config;
    public UniversalSpawner<Deposit> Spawner;
}