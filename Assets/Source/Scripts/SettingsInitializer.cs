using Zenject;

public class SettingsInitializer
{
    private readonly SaveService _saveService;
    private readonly AudioVolumeSettings _audioSettings;

    public SettingsInitializer(SaveService saveService, AudioVolumeSettings audioSettings)
    {
        _saveService = saveService;
        _audioSettings = audioSettings;
    }

    public void Initialize()
    {
        _audioSettings.Initialize(_saveService.LoadAudioSettingsData());
    }
}