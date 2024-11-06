using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterStatsManager
{
    private const string SaveKey = "CharacterStats";

    public static CharacterStats CurrentStats { get; private set; }

    public static void InitializeCharacterStats()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            CurrentStats = LoadCharacterStats();
        }
        else
        {
            CurrentStats = new CharacterStats
            {
                attack = new CharacterStat(5),
                health = new CharacterStat(10),
                speed = new CharacterStat(3),
                point = new CharacterStat(5000)
            };
        }

        SaveCharacterStats(CurrentStats);
    }
    public static void SaveCharacterStats(CharacterStats characterStats)
    {
        string json = JsonUtility.ToJson(characterStats);
        string encodeJson = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));

        PlayerPrefs.SetString(SaveKey, encodeJson);
        PlayerPrefs.Save();
    }

    public static CharacterStats LoadCharacterStats()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            Debug.Log("No saved data found");
            return null;
        }


        string encodedJson = PlayerPrefs.GetString(SaveKey);
        string json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedJson));
        return JsonUtility.FromJson<CharacterStats>(json);
    }

    public static void ClearCharacterStats()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }
}
