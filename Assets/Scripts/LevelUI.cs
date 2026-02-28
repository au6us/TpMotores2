using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Header("Barra única de tiempo")]
    public Image starBar;

    [Header("Estrellas (3 imágenes)")]
    public Image star3;
    public Image star2;
    public Image star1;

    [Header("Tiempo total para vaciar todo (segundos)")]
    public float maxTime = 100f;

    private float startTime;
    private float starTime;

    void Start()
    {
        startTime = Time.time;
        starTime = maxTime / 3f;

        if (starBar != null) starBar.fillAmount = 1f;

        star3.fillAmount = 1f;
        star2.fillAmount = 1f;
        star1.fillAmount = 1f;
    }

    void Update()
    {
        float elapsed = Time.time - startTime;

        if (starBar != null)
        {
            float fillValue = 1f - (elapsed / maxTime);
            starBar.fillAmount = Mathf.Clamp01(fillValue);
        }

        if (elapsed < starTime)
        {
            float t = elapsed / starTime;
            star3.fillAmount = 1f - t;
            star2.fillAmount = 1f;
            star1.fillAmount = 1f;
        }
        else if (elapsed < starTime * 2f)
        {
            star3.fillAmount = 0f;
            float t = (elapsed - starTime) / starTime;
            star2.fillAmount = 1f - t;
            star1.fillAmount = 1f;
        }
        else if (elapsed < starTime * 3f)
        {
            star3.fillAmount = 0f;
            star2.fillAmount = 0f;
            float t = (elapsed - starTime * 2f) / starTime;
            star1.fillAmount = 1f - t;
        }
        else
        {
            star3.fillAmount = 0f;
            star2.fillAmount = 0f;
            star1.fillAmount = 0f;
        }
    }
}