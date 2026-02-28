using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public int life;
    public int maxLife;
    [SerializeField] Controller controller;
    public float speed = 5;
    [SerializeField] float smoothedMove;
    private bool lookRight = true;
    public GameplayManager gamePlayCanvas;

    [Header("Salto")]
    [SerializeField] LayerMask floor;
    [SerializeField] Transform floorController;
    [SerializeField] Vector3 boxDimensions;
    public float jumpForce = 5f;
    private bool isGrounded = false;

    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown = 3f;
    private float dashCooldownTimer = 0f;
    private bool isDashing = false;
    private float starterGravity;
    private bool canDash = true;
    private bool canMove = true;
    private bool dashUnlocked = true;

    [Header("Knockback")]
    [SerializeField] public Vector2 knockBackSpeed;
    [SerializeField] private float timeLostControl;

    [Header("DoubleJump / TripleJump")]
    [SerializeField] private int saltosExtraRestantes;
    [SerializeField] private int saltosExtra = 1; // Por defecto 1 (Doble Salto)

    [Header("Rebote")]
    [SerializeField] float speedRebound;

    [Header("SaltoPared")]
    [SerializeField] private Transform controladorPared;
    [SerializeField] private Vector3 dimensionCajaPared;
    [SerializeField] private bool enPared;
    [SerializeField] private bool deslizando;
    [SerializeField] private float velocidadDeslizar;
    [SerializeField] private float fuerzaSaltoParedX;
    [SerializeField] private float fuerzaSaltoParedY;
    [SerializeField] private float tiempoSaltoPared;
    private bool saltandoDePared;

    [Header("Animaciones")]
    private Animator animator;

    [SerializeField] private ParticleSystem particulasDash;
    [SerializeField] private ParticleSystem particulasCorrer;
    [SerializeField] private ParticleSystem particulasAterrizaje;
    [SerializeField] private ParticleSystem particulasDj;

    private bool wasGrounded = true;

    [SerializeField] private BarraDeVida barraDeVida;

    [Header("Checkpoint")]
    [SerializeField] private CheckpointManager checkpointManager;

    [Header("Sonidos Player")]
    [SerializeField] private AudioSource jumpAudioSource;
    [SerializeField] private AudioSource doubleJumpAudioSource;
    [SerializeField] private AudioSource dashAudioSource;
    [SerializeField] private AudioSource WalkAudioSource;
    [SerializeField] private AudioSource HitAudioSource;

    [Header("Save system")]
    private int coins;
    private JSONSaveHandler saveHandler;
    [SerializeField] private TextMeshProUGUI coinCounterText;

    public ButtonController buttonController;

    private void Start()
    {
        Application.targetFrameRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value) + 5;

        life = maxLife;
        barraDeVida.InicializarBarraDeVida(life);
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        starterGravity = rb.gravityScale;
        checkpointManager = FindObjectOfType<CheckpointManager>();
        life = maxLife;
        if (checkpointManager != null)
        {
            checkpointManager.UpdateCheckpointPosition(transform.position);
        }

        saveHandler = FindObjectOfType<JSONSaveHandler>();
        if (saveHandler != null)
        {
            coins = saveHandler.LoadData();

            if (saveHandler.LoadTripleJumpState())
            {
                saltosExtra = 2;
            }
        }

        UpdateCoinUI();

        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        if (currentLevel == 1)
        {
            dashUnlocked = true;
        }
        else
        {
            if (saveHandler != null)
            {
                dashUnlocked = saveHandler.LoadDashState();
            }
            else
            {
                dashUnlocked = false;
            }
        }

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

        ButtonController controllerObj = FindObjectOfType<ButtonController>();
        if (controllerObj != null)
        {
            controllerObj.SetDashButtonState(dashUnlocked);
        }
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapBox(floorController.position, boxDimensions, 0f, floor);
        enPared = Physics2D.OverlapBox(controladorPared.position, dimensionCajaPared, 0f, floor);

        if (!canDash)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0)
            {
                canDash = true;
            }
        }

        if (!wasGrounded && isGrounded)
        {
            saltosExtraRestantes = saltosExtra;
            animator.SetBool("isDoubleJumping", false);
            particulasAterrizaje.Play();
        }

        wasGrounded = isGrounded;

        animator.SetBool("enSuelo", isGrounded);

        if (enPared && !isGrounded)
        {
            if (controller.GetMoveDir().x != 0)
            {
                deslizando = true;
                animator.SetBool("Deslizando", deslizando);
            }
            else
            {
                deslizando = false;
                animator.SetBool("Deslizando", false);
            }
        }
        else
        {
            deslizando = false;
            animator.SetBool("Deslizando", false);
        }

        if (deslizando)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -velocidadDeslizar, float.MaxValue));
        }
    }


    public void Knockback(Vector2 punchPoint)
    {
        rb.velocity = new Vector2(-knockBackSpeed.x * punchPoint.x, knockBackSpeed.y);
        HitAudioSource.Play();
    }

    private void FixedUpdate()
    {
        if (!saltandoDePared)
        {
            if (canMove)
            {
                rb.velocity = new Vector2(controller.GetMoveDir().x * speed, rb.velocity.y);
                Move();
            }
        }

        if (controller.GetMoveDir().x != 0 && isGrounded)
        {
            if (!particulasCorrer.isPlaying)
            {
                particulasCorrer.Play();
            }

            if (!WalkAudioSource.isPlaying)
            {
                WalkAudioSource.Play();
            }
        }
        else
        {
            if (particulasCorrer.isPlaying)
            {
                particulasCorrer.Stop();
            }

            if (WalkAudioSource.isPlaying)
            {
                WalkAudioSource.Stop();
            }
        }

        if (dashUnlocked)
        {
            if (controller.IsDashing() && canDash && !isDashing)
            {
                StartCoroutine(Dash());
                particulasDash.Play();
                dashAudioSource.Play();
            }
        }

        animator.SetFloat("Horizontal", Mathf.Abs(rb.velocity.x));
        animator.SetBool("isDoubleJumping", false);
        animator.SetFloat("VelocidadY", rb.velocity.y);

        if (controller.IsJumping())
        {
            if (isGrounded && !deslizando)
            {
                Jump();
                jumpAudioSource.Play();
            }
            else if (enPared && deslizando)
            {
                SaltoPared();
            }
            else
            {
                if (saltosExtraRestantes > 0)
                {
                    Jump();
                    saltosExtraRestantes -= 1;
                    animator.SetBool("isDoubleJumping", true);
                    particulasDj.Play();
                    doubleJumpAudioSource.Play();
                }
            }
        }

    }

    private void Move()
    {
        float move = rb.velocity.x;

        if (move > 0 && !lookRight)
        {
            Turn();
        }
        else if (move < 0 && lookRight)
        {
            Turn();
        }
    }

    private void Turn()
    {
        lookRight = !lookRight;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void SaltoPared()
    {
        enPared = false;
        rb.velocity = new Vector2(fuerzaSaltoParedX * -controller.GetMoveDir().x, fuerzaSaltoParedY);
        StartCoroutine(CambioSaltoPared());
    }

    IEnumerator CambioSaltoPared()
    {
        saltandoDePared = true;
        yield return new WaitForSeconds(tiempoSaltoPared);
        saltandoDePared = false;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canMove = false;
        canDash = false;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(dashSpeed * transform.localScale.x, rb.velocity.y);

        yield return new WaitForSeconds(dashTime);

        canMove = true;
        rb.gravityScale = starterGravity;

        yield return new WaitForSeconds(dashCooldown);
        dashCooldownTimer = dashCooldown;

        isDashing = false;
    }

    public void Rebound()
    {
        rb.velocity = new Vector2(rb.velocity.x, speedRebound);
    }

    public void TakeDamage(int value, Vector2 posicion)
    {
        life -= value;
        barraDeVida.CambiarVidaActual(life);
        animator.SetTrigger("Golpe");
        StartCoroutine(LostControl());
        StartCoroutine(CollisionDesactive());
        Knockback(posicion);

        if (life <= 0)
        {
            life = 0;
            StartCoroutine(SecuenciaMuerteAutomatica());
        }
    }

    private IEnumerator SecuenciaMuerteAutomatica()
    {
        yield return new WaitForSeconds(0f);

        Dead();

        RespawnAtCheckpoint();
        
    }

    private IEnumerator CollisionDesactive()
    {
        Physics2D.IgnoreLayerCollision(6, 8, true);
        yield return new WaitForSeconds(timeLostControl);
        Physics2D.IgnoreLayerCollision(6, 8, false);
    }
    private IEnumerator LostControl()
    {
        canMove = false;
        yield return new WaitForSeconds(timeLostControl);
        canMove = true;
    }

    public void ResetPlayerCollisions()
    {
        Physics2D.IgnoreLayerCollision(6, 8, false);
    }

    public void Curar(int cantidadCuracion)
    {
        life += cantidadCuracion;

        if (life > maxLife)
        {
            life = maxLife;
        }

        barraDeVida.CambiarVidaActual(life);
    }

    private void Dead()
    {
        // Cargamos el número de muertes que tenemos guardado (si no hay nada, empieza en 0)
        int muertesGuardadas = PlayerPrefs.GetInt("ContadorMuertes", 0);

        // Sumamos la nueva muerte
        muertesGuardadas++;

        // Verificamos si toca anuncio
        if (muertesGuardadas % 2 == 0)
        {
            if (AdsManager.Instance != null)
            {
                AdsManager.Instance.ShowInterstitial();
            }
        }

        //Guardamos el nuevo valor para la próxima vez
        PlayerPrefs.SetInt("ContadorMuertes", muertesGuardadas);
        PlayerPrefs.Save();
    }


    public void RespawnAtCheckpoint()
    {
        if (checkpointManager != null)
        {
            rb.velocity = Vector2.zero;
            transform.position = checkpointManager.GetCheckpointPosition();
            life = maxLife;
            barraDeVida.InicializarBarraDeVida(life);
            Debug.Log("Jugador respawneado en checkpoint: " + transform.position);
        }
    }

    public void CollectCoin()
    {
        coins++;
        if (saveHandler != null)
        {
            saveHandler.AddCoins(1);
        }
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        coinCounterText.text = coins.ToString();
    }

    public int GetCoins()
    {
        return coins;
    }

    public void SetDashUnlocked(bool unlocked)
    {
        dashUnlocked = unlocked;
    }

    public void UnlockDash()
    {
        dashUnlocked = true;
        saveHandler.SaveDashState(dashUnlocked);

        ButtonController controllerObj = FindObjectOfType<ButtonController>();
        if (controllerObj != null)
        {
            controllerObj.SetDashButtonState(dashUnlocked);
        }
    }

    public void UnlockTripleJump()
    {
        saltosExtra = 2; 
        saltosExtraRestantes = saltosExtra;
    }

    private void OnDrawGizmos()
    {
        if (floorController != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(floorController.position, boxDimensions);
            Gizmos.DrawWireCube(controladorPared.position, dimensionCajaPared);
        }
    }

    public void SilenciarAudio()
    {
        if (WalkAudioSource != null && WalkAudioSource.isPlaying)
        {
            WalkAudioSource.Stop();
        }
    }
}