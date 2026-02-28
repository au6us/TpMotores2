using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : Controller
{
    private Vector3 dir;
    private bool jump = false;
    private bool dash = false;
    private bool moviendoDerecha = false;
    private bool moviendoIzquierda = false;

    private void Start()
    {
        int currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        if (currentLevel == 2)
        {
            var saveHandler = FindObjectOfType<JSONSaveHandler>();
            if (saveHandler != null)
            {
                SetDashButtonState(saveHandler.LoadDashState());
            }
        }
    }

    private void Update()
    {
        // 1. Detectamos Teclado (PC)
        bool tecladoD = Input.GetKey(KeyCode.D);
        bool tecladoA = Input.GetKey(KeyCode.A);

        // 2. Combinamos: Se mueve si apretás Tecla O si mantenés el Botón Táctil
        float horizontalInput = 0;

        if (tecladoD || moviendoDerecha)
        {
            horizontalInput = 1;
        }
        else if (tecladoA || moviendoIzquierda)
        {
            horizontalInput = -1;
        }

        dir = new Vector3(horizontalInput, 0, 0);

        // 3. Inputs de acción (W y S)
        if (Input.GetKeyDown(KeyCode.W)) PressJumpButton();
        if (Input.GetKeyDown(KeyCode.S)) PressDashButton();
    }

    public void PressRightButton()
    {
        moviendoDerecha = true;
    }

    public void ReleaseRightButton()
    {
        moviendoDerecha = false;
    }

    public void PressLeftButton()
    {
        moviendoIzquierda = true;
    }

    public void ReleaseLeftButton()
    {
        moviendoIzquierda = false;
    }

    public void stopMovement()
    {
        moviendoDerecha = false;
        moviendoIzquierda = false;
        dir = Vector3.zero; // Agregamos esto para forzar la detención al instante
    }

    private void OnDisable()
    {
        stopMovement();
    }

    
    public override Vector3 GetMoveDir()
    {
        return dir;
    }

    public override bool IsJumping()
    {
        bool wasJumping = jump;
        jump = false;
        return wasJumping;
    }

    public override bool IsDashing()
    {
        bool wasDashing = dash;
        dash = false;
        return wasDashing;
    }

    public void PressJumpButton() { jump = true; }
    public void PressDashButton() { dash = true; }

    public void SetDashButtonState(bool unlocked)
    {
        // Se desactiva o activa visualmente el botón de dash
        GameObject btnDash = GameObject.Find("DashButton"); 
        if (btnDash != null) btnDash.SetActive(unlocked);
    }
}