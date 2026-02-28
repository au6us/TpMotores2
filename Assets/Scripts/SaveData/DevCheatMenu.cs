using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DevCheatMenu : MonoBehaviour
{
    [Header("UI del Menú Secreto")]
    [SerializeField] private GameObject panelDev;
    [SerializeField] private TMP_InputField inputMonedas; 
    [SerializeField] private Button btnAplicar;
    [SerializeField] private Button btnCerrar;

    private JSONSaveHandler saveHandler;

    private void Start()
    {
        saveHandler = FindObjectOfType<JSONSaveHandler>();

        if (panelDev != null) panelDev.SetActive(false);

        btnAplicar.onClick.AddListener(AplicarMonedas);
        btnCerrar.onClick.AddListener(CerrarPanel);
    }

    private void Update()
    {
        // Detecta si las teclas A, W y D están presionadas AL MISMO TIEMPO
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
        {
            if (panelDev != null && !panelDev.activeSelf)
            {
                panelDev.SetActive(true); 
            }
        }
    }

    private void AplicarMonedas()
    {
        if (saveHandler != null)
        {
            
            if (int.TryParse(inputMonedas.text, out int cantidadMonedas))
            {
                
                saveHandler.SaveData(cantidadMonedas);

                Debug.Log("¡Truco activado! Monedas seteadas a: " + cantidadMonedas);

                // Recargamos el menú rápido para que los numeritos de la UI se actualicen solos
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                Debug.LogWarning("Che, escribí solo números en el panel.");
            }
        }
    }

    private void CerrarPanel()
    {
        panelDev.SetActive(false);
    }
}