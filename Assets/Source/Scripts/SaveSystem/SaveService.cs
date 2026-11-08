using System;

public class SaveService
{
    private GameSaveData _gameSaveData;
    private SettingsSaveData _settingsSaveData;

    public SaveService(GameSaveData initialData, SettingsSaveData settingsSaveData)
    {
        _gameSaveData = initialData;
        _settingsSaveData = settingsSaveData;
    }

    public void SaveGameData(GameSaveData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        _gameSaveData = data;
    }

    public GameSaveData LoadGameData()
    {
        return _gameSaveData;
    }

    public void SaveAudioSettings(AudioSettingsSaveData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        _settingsSaveData.SetAudioSettings(data);
    }

    public AudioSettingsSaveData LoadAudioSettingsData()
    {
        return _settingsSaveData.AudioSettingsSaveData;
    }
}
