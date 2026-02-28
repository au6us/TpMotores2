using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int levelIndex;
    private float levelStartTime;
    private JSONSaveHandler saveHandler;

    void Start()
    {
        levelStartTime = Time.time;
        saveHandler = FindObjectOfType<JSONSaveHandler>();

        Debug.Log($"Cargando datos del nivel {levelIndex}. Estrellas actuales: {LoadStars()}");
    }

    public void CompleteLevel()
    {
        float elapsedTime = Time.time - levelStartTime;
        int starsEarned = CalculateStars(elapsedTime);

        Debug.Log($"Nivel {levelIndex} completado en {elapsedTime} segundos. Estrellas ganadas: {starsEarned}");

        saveHandler.SaveStars(levelIndex, starsEarned);
    }

    //Se calcula el tiempo en que se gana cada estrella
    int CalculateStars(float time)
    {
        if (time <= 40f)
            return 3;
        else if (time <= 80f)
            return 2;
        else if (time <= 120f)
            return 1;
        else
            return 0;
    }

    public int LoadStars()
    {
        return saveHandler.LoadStars(levelIndex);
    }
}
