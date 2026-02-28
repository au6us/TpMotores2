using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuCoinDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinCounterText;
    private JSONSaveHandler saveSystem;

    private void Start()
    {
        // Busca el JSON en la escena del menú
        saveSystem = FindObjectOfType<JSONSaveHandler>();
        if (saveSystem == null)
        {
            GameObject saveSystemObject = new GameObject("JSONSaveHandler");
            saveSystem = saveSystemObject.AddComponent<JSONSaveHandler>();
        }

        // Carga las monedas y actualiza el texto del contador
        int coins = saveSystem.LoadData();
        UpdateCoinUI(coins);
    }

    private void UpdateCoinUI(int coins)
    {
        coinCounterText.text = coins.ToString();
    }
}
