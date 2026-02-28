using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class JSONSaveHandler : MonoBehaviour
{
    private string filePath;
    private string savePath;
    private const string DashKey = "DashUnlocked";
    private const string TripleJumpKey = "TripleJumpUnlocked";

    [System.Serializable]
    public class PlayerData
    {
        public int coins;
        public int starsBought;
    }

    [System.Serializable]
    private class LevelDataWrapper
    {
        public List<int> levelIndices;
        public List<int> starCounts;

        public LevelDataWrapper(Dictionary<int, int> levelStars)
        {
            levelIndices = new List<int>(levelStars.Keys);
            starCounts = new List<int>(levelStars.Values);
        }

        public Dictionary<int, int> ToDictionary()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            for (int i = 0; i < levelIndices.Count; i++)
            {
                dictionary[levelIndices[i]] = starCounts[i];
            }
            return dictionary;
        }
    }

    private void Awake()
    {
        filePath = Application.persistentDataPath + "/playerData.json";
        savePath = Application.persistentDataPath + "/level_data.json";
    }

    public int LoadData()
    {
        return LoadPlayerData().coins;
    }

    public void SaveData(int coins)
    {
        PlayerData current = LoadPlayerData();
        current.coins = coins;
        SavePlayerData(current.coins, current.starsBought);
    }

    public int GetCoins()
    {
        return LoadData();
    }

    public void AddCoins(int amount)
    {
        int current = LoadData();
        SaveData(current + amount);
    }

    // Función interna para guardar todo en el JSON
    public void SavePlayerData(int coins, int starsBought)
    {
        PlayerData data = new PlayerData
        {
            coins = coins,
            starsBought = starsBought
        };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
    }

    public PlayerData LoadPlayerData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        return new PlayerData { coins = 0, starsBought = 0 };
    }

    public int GetBoughtStars()
    {
        return LoadPlayerData().starsBought;
    }

    public int LoadStarsBought()
    {
        return GetBoughtStars();
    }

    public void SaveBoughtStars(int stars)
    {
        PlayerData current = LoadPlayerData();
        SavePlayerData(current.coins, stars);
    }


    public void SaveStars(int levelIndex, int stars)
    {
        Dictionary<int, int> levelStars = LoadAllStars();

        // Guardamos si no existe o si es un puntaje mejor
        if (!levelStars.ContainsKey(levelIndex) || stars > levelStars[levelIndex])
        {
            levelStars[levelIndex] = stars;
            string json = JsonUtility.ToJson(new LevelDataWrapper(levelStars));
            File.WriteAllText(savePath, json);
        }
    }

    public int LoadLevelStars(int levelIndex)
    {
        return LoadStars(levelIndex);
    }

    public int LoadStars(int levelIndex)
    {
        Dictionary<int, int> levelStars = LoadAllStars();
        return levelStars.ContainsKey(levelIndex) ? levelStars[levelIndex] : 0;
    }

    private Dictionary<int, int> LoadAllStars()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            LevelDataWrapper dataWrapper = JsonUtility.FromJson<LevelDataWrapper>(json);
            return dataWrapper.ToDictionary();
        }
        return new Dictionary<int, int>();
    }

    public int GetTotalStars()
    {
        Dictionary<int, int> levelStars = LoadAllStars();
        int total = 0;
        foreach (var stars in levelStars.Values)
        {
            total += stars;
        }
        // Sumar las estrellas compradas
        total += GetBoughtStars();
        return total;
    }

    public void SaveDashState(bool isUnlocked)
    {
        PlayerPrefs.SetInt(DashKey, isUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool LoadDashState()
    {
        return PlayerPrefs.GetInt(DashKey, 0) == 1;
    }

    public void DeleteData()
    {

        if (File.Exists(filePath))File.Delete(filePath);
        if (File.Exists(savePath)) File.Delete(savePath);
        PlayerPrefs.DeleteKey(DashKey);
        PlayerPrefs.DeleteKey(TripleJumpKey);
        Debug.Log("Datos borrados");
    }

    public void SaveTripleJumpState(bool isUnlocked)
    {
        PlayerPrefs.SetInt(TripleJumpKey, isUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool LoadTripleJumpState()
    {
        return PlayerPrefs.GetInt(TripleJumpKey, 0) == 1;
    }
}