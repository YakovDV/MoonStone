using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class LocalizationService
{
    private struct Translation
    {
        public string Ru;
        public string En;
        public string Tr;
    }

    private readonly Dictionary<string, Translation> _translations = new();

    public string Language { get; private set; } = "en";

    public event Action LanguageChanged;

    public LocalizationService(LocalizationTable table)
    {
        if (table == null)
        {
            Debug.LogError("LocalizationTable is missing.");
            return;
        }

        Load(table.Data);
    }

    private void Load(string data)
    {
        var lines = data.Split('\n');

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd('\r');

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                continue;

            var cells = line.Split('\t');

            if (cells.Length != 4)
            {
                Debug.LogWarning($"Invalid localization row: {line}");
                continue;
            }

            string key = cells[0].Trim();

            if (key == "key" || string.IsNullOrEmpty(key))
                continue;

            if (_translations.ContainsKey(key))
            {
                Debug.LogWarning($"Duplicate localization key: {key}");
                continue;
            }

            _translations.Add(key, new Translation
            {
                Ru = cells[1].Trim(),
                En = cells[2].Trim(),
                Tr = cells[3].Trim()
            });
        }
    }

    public string Get(string key)
    {
        if (string.IsNullOrEmpty(key))
            return string.Empty;

        if (_translations.TryGetValue(key, out var translation) == false)
        {
            Debug.LogWarning($"Missing localization: {key}");
            return $"[{key}]";
        }

        string value = Language switch
        {
            "ru" => translation.Ru,
            "tr" => translation.Tr,
            _ => translation.En
        };

        if (string.IsNullOrEmpty(value) == false)
            return value;

        if (string.IsNullOrEmpty(translation.En) == false)
            return translation.En;

        if (string.IsNullOrEmpty(translation.Ru) == false)
            return translation.Ru;

        return $"[{key}]";
    }

    public void SetLanguage(string language)
    {
        string newLanguage = language?.ToLowerInvariant() switch
        {
            "ru" => "ru",
            "tr" => "tr",
            _ => "en"
        };

        if (Language == newLanguage)
            return;

        Language = newLanguage;
        LanguageChanged?.Invoke();
    }
}
