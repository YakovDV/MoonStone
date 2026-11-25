using System;

public class SaveService
{
    private GameSaveData _gameSaveData;
    private SettingsSaveData _settingsSaveData;
    private ISaveStorage _saveStorage;

    public SaveService(GameSaveData initialData, SettingsSaveData settingsSaveData)
    {
        _gameSaveData = initialData;
        _settingsSaveData = settingsSaveData;
    }

    public void Initialize(ISaveStorage saveStorage)
    {
        if (saveStorage.TryLoad(out GameSaveData gameData, out SettingsSaveData settingsSaveData))
        {
            _gameSaveData = gameData;
            _settingsSaveData = settingsSaveData;
        }
    }

    public void SaveGameData(GameSaveData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        _gameSaveData = data;

        _saveStorage.Save(_gameSaveData, _settingsSaveData);
    }

    public GameSaveData LoadGameData()
    {
        return _gameSaveData;
    }

    public int GetProgress()
    {
        return _gameSaveData.TotalProgress;
    }

    public void SaveAudioSettings(AudioSettingsSaveData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        _settingsSaveData.SetAudioSettings(data);

        _saveStorage.Save(_gameSaveData, _settingsSaveData);
    }

    public AudioSettingsSaveData LoadAudioSettingsData()
    {
        return _settingsSaveData.AudioSettingsSaveData;
    }
}