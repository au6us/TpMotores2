using UnityEngine;
using UnityEngine.UI;
using System;

public class ConfirmPopup : MonoBehaviour
{
    public static ConfirmPopup Instance;

    [Header("Referencias UI")]
    [SerializeField] private GameObject panelPopup;
    [SerializeField] private Button btnConfirmar; 
    [SerializeField] private Button btnCancelar;  

    private Action accionConfirmar;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        panelPopup.SetActive(false);

        // Se asignan las funciones por código
        btnConfirmar.onClick.AddListener(OnConfirmarClic);
        btnCancelar.onClick.AddListener(OcultarPopup);
    }

    public void MostrarPopup(Action accion)
    {
        accionConfirmar = accion;
        panelPopup.SetActive(true);
    }

    private void OnConfirmarClic()
    {
        if (accionConfirmar != null)
        {
            accionConfirmar.Invoke();
        }
        OcultarPopup();
    }

    public void OcultarPopup()
    {
        panelPopup.SetActive(false);
        accionConfirmar = null;
    }
}