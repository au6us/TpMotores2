using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    private Player player;
    void Start()
    {
        player = FindObjectOfType<Player>();
    }
    public void Pause()
    {
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }
    public void Reanudate()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void ConfirmRestart()
    {
        ConfirmPopup.Instance.MostrarPopup(Restart);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ConfirmMenu()
    {
        ConfirmPopup.Instance.MostrarPopup(Menu);
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
        player.ResetPlayerCollisions();
    }
    public void Quit()
    {
        Application.Quit();
    }
}
