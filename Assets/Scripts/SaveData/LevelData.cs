using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelData : MonoBehaviour
{
    public List<int> levelIndices;
    public List<int> starCounts;

    private JSONSaveHandler saveHandler;

    void Start()
    {
        saveHandler = FindObjectOfType<JSONSaveHandler>();
        LoadLevelData();
    }

    void LoadLevelData()
    {
        if (saveHandler != null)
        {
            Debug.Log("Total de estrellas disponibles: " + saveHandler.GetTotalStars());
        }
    }

    // Método para obtener todas las estrellas de niveles específicos
    public int GetStarsForLevel(int levelIndex)
    {
        if (saveHandler != null)
        {
            return saveHandler.LoadStars(levelIndex);
        }
        return 0;
    }
}