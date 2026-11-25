using YG;

public class YgSaveStorage : ISaveStorage
{
    public bool TryLoad(out GameSaveData gameData, out SettingsSaveData settingsData)
    {
        gameData = YG2.saves.GameData;
        settingsData = YG2.saves.SettingsData;

        return YG2.saves.HasSave && gameData != null && settingsData != null;
    }

    public void Save(GameSaveData gameData, SettingsSaveData settingsData)
    {
        YG2.saves.GameData = gameData;
        YG2.saves.SettingsData = settingsData;
        YG2.saves.HasSave = true;

        YG2.SaveProgress();
    }
}

public interface ISaveStorage
{
    bool TryLoad(out GameSaveData gameData, out SettingsSaveData settingsData);

    void Save(GameSaveData gameData, SettingsSaveData settingsData);
}