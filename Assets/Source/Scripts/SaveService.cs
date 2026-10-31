using System;

public class SaveService
{
    private SaveData _saveData;

    public SaveService(SaveData initialData)
    {
        _saveData = initialData;
    }

    public void Save(SaveData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        _saveData = data;
    }

    public SaveData Load()
    {
        return _saveData;
    }
}