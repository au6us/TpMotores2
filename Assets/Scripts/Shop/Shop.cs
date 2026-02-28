using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    [Header("--- UI ---")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private UIWarning warningSinPlata;

    [Header("--- Botones ---")]
    [SerializeField] private Button btnDash;
    [SerializeField] private Button btnItem2; // Estrella x1
    [SerializeField] private Button btnTripleSalto;
    [SerializeField] private Button btnEnergia;

    [Header("--- Precios ---")]
    [SerializeField] private int precioDash = 10;
    [SerializeField] private int precioItem2 = 5;
    [SerializeField] private int precioTripleSalto = 100;
    [SerializeField] private int precioEnergia = 20;

    private JSONSaveHandler saveHandler;
    private int currentCoins;
    private int currentStarsBought;

    private void Start()
    {
        saveHandler = FindObjectOfType<JSONSaveHandler>();

        if (saveHandler != null)
        {
            currentCoins = saveHandler.GetCoins();
            currentStarsBought = saveHandler.GetBoughtStars();

            // Bloquea el Dash si ya lo tenés
            if (saveHandler.LoadDashState())
            {
                BloquearBoton(btnDash);
            }

            // Bloquea el Triple Salto si ya lo tenés
            if (saveHandler.LoadTripleJumpState())
            {
                BloquearBoton(btnTripleSalto);
            }
        }
        UpdateUI();
    }

    public void ComprarDash()
    {
        if (saveHandler == null) return;
        if (saveHandler.LoadDashState()) return;

        if (currentCoins >= precioDash)
        {
            ConfirmPopup.Instance.MostrarPopup(EjecutarCompraDash);
        }
        else
        {
            MostrarErrorDinero();
        }
    }

    private void EjecutarCompraDash()
    {
        currentCoins -= precioDash;
        saveHandler.SavePlayerData(currentCoins, currentStarsBought);
        saveHandler.SaveDashState(true);

        FindObjectOfType<Player>()?.UnlockDash();
        UpdateUI();
        BloquearBoton(btnDash);
    }

    public void ComprarEnergia()
    {
        if (saveHandler == null) return;

        if (currentCoins >= precioEnergia)
        {
            ConfirmPopup.Instance.MostrarPopup(EjecutarCompraEnergia);
        }
        else
        {
            MostrarErrorDinero();
        }
    }

    private void EjecutarCompraEnergia()
    {
        StaminaSystem staminaSystem = FindObjectOfType<StaminaSystem>();

        if (staminaSystem != null)
        {
            currentCoins -= precioEnergia;
            saveHandler.SavePlayerData(currentCoins, currentStarsBought);

            staminaSystem.RechargeStamina(5);

            UpdateUI();
        }
    }

    public void ComprarItem2()
    {
        if (currentCoins >= precioItem2)
        {
            ConfirmPopup.Instance.MostrarPopup(EjecutarCompraItem2);
        }
        else
        {
            MostrarErrorDinero();
        }
    }

    private void EjecutarCompraItem2()
    {
        currentCoins -= precioItem2;
        currentStarsBought += 1;

        saveHandler.SavePlayerData(currentCoins, currentStarsBought);
        UpdateUI();
    }

    public void ComprarTripleSalto()
    {
        if (saveHandler == null) return;
        if (saveHandler.LoadTripleJumpState()) return;

        if (currentCoins >= precioTripleSalto)
        {
            ConfirmPopup.Instance.MostrarPopup(EjecutarCompraTripleSalto);
        }
        else
        {
            MostrarErrorDinero();
        }
    }

    private void EjecutarCompraTripleSalto()
    {
        currentCoins -= precioTripleSalto;
        saveHandler.SavePlayerData(currentCoins, currentStarsBought);

        // Guardamos que se compró en el JSON
        saveHandler.SaveTripleJumpState(true);

        // Activamos el poder en el Player
        FindObjectOfType<Player>()?.UnlockTripleJump();

        UpdateUI();
        BloquearBoton(btnTripleSalto); // Bloqueamos para que no lo compre de nuevo
    }

    private void MostrarErrorDinero()
    {
        if (warningSinPlata != null)
        {
            warningSinPlata.MostrarAviso();
        }
    }

    private void UpdateUI()
    {
        if (coinsText != null) coinsText.text = "" + currentCoins;

        if (starsText != null && saveHandler != null)
        {
            int starsLevel1 = saveHandler.LoadLevelStars(1);
            int total = starsLevel1 + currentStarsBought;
            starsText.text = "Estrellas Totales: " + total;
        }
    }

    private void BloquearBoton(Button boton)
    {
        if (boton != null)
        {
            boton.interactable = false;
        }
    }

    public void Back()
    {
        SceneManager.LoadScene(0);
    }
}