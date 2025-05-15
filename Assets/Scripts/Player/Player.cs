using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public int life;
    public int maxLife;
    [SerializeField] Controller controller;
    [SerializeField] float speed = 5;
    [SerializeField] float smoothedMove;
    private bool lookRight = true;
    public GameplayManager gamePlayCanvas;

    [Header("Salto")]
    [SerializeField] LayerMask floor;
    [SerializeField] Transform floorController;
    [SerializeField] Vector3 boxDimensions;
    [SerializeField] float jumpForce = 5f;
    private bool isGrounded = false;

    [Header("Dash")]
    [SerializeField] float dashSpeed;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown = 3f;  
    private float dashCooldownTimer = 0f;
    private bool isDashing = false;
    private float starterGravity;
    private bool canDash = true;
    private bool canMove = true; //También ayuda al knowback cuando choca con algún enemy
    private bool dashUnlocked = true; // Inicialmente desbloqueado para el nivel 1

    [Header("Knockback")]
    [SerializeField] public Vector2 knockBackSpeed; //Knockback
    [SerializeField] private float timeLostControl; //Knockback

    [Header("DoubleJump")]
    [SerializeField] private int saltosExtraRestantes;
    [SerializeField] private int saltosExtra;

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

    // Para las partículas cuando corre/camina/dash
    [SerializeField] private ParticleSystem particulasDash;
    [SerializeField] private ParticleSystem particulasCorrer;
    [SerializeField] private ParticleSystem particulasAterrizaje;
    [SerializeField] private ParticleSystem particulasDj;

    private bool wasGrounded = true; // Para que cuando toque el suelo, aparezcan las partículas de aterrizaje

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
    private JSONSaveHandler saveSystem;
    [SerializeField] private TextMeshProUGUI coinCounterText;

    public ButtonController buttonController;

    // GRUPO 2 – ORDERBY: visión de la moneda más cercana
    [Header("Visión Monedas")]
    [SerializeField] private float coinMaxXOffset = 5f;
    [SerializeField] private float coinMaxYOffset = 1f;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI distanceTextMts;

    // GRUPO 3 – ALL: recompensa cada 3 monedas
    [Header("Efecto Curación")]
    [SerializeField] private AudioSource healAudioSource;
    [SerializeField] private AudioClip healClip;
    private int coinsCollectedForHeal;
    private const int coinsForHeal = 3;

    private void Start()
    {
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
        saveSystem = FindObjectOfType<JSONSaveHandler>();
        coins = saveSystem.LoadData(); // Cargar las monedas al inicio
        UpdateCoinUI();
        //dashUnlocked = saveSystem.LoadDashState();

        int currentLevel = SceneManager.GetActiveScene().buildIndex; // Obtener el nivel actual
        if (currentLevel == 1)
        {
            dashUnlocked = true; // En el nivel 1, el dash siempre está desbloqueado
        }
        else if (currentLevel == 2)
        {
            dashUnlocked = saveSystem.LoadDashState(); // En el nivel 2, cargar desde el sistema de guardado
        }

        // Cargar el estado del dash
        //dashUnlocked = saveSystem.LoadDashState();

        // Configurar el estado del botón de dash
        ButtonController controller = FindObjectOfType<ButtonController>();
        if (controller != null)
        {
            controller.SetDashButtonState(dashUnlocked);
        }

        StartCoroutine(CheckNearbyEnemiesCoroutine());
    }

    private void Update()
    {
        // —— LINQ OrderBy ——
        var nearest = GetClosestCoinInBox(coinMaxXOffset, coinMaxYOffset);
        if (nearest.coin != null)
        {
            distanceText.text = $"{nearest.distance:F2}";
            distanceTextMts.text = "MTS";
        }
        else
        {
            distanceText.text = "";
            distanceTextMts.text = "";
        }

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

        // Determina si se debe deslizar en la pared
        if (enPared && !isGrounded)
        {
            if (controller.GetMoveDir().x != 0) // Solo deslizar si se está moviendo
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


    public void CollectCoin()
    {
        coins++;
        saveSystem.SaveData(coins);
        UpdateCoinUI();

        // —— LINQ All ——
        coinsCollectedForHeal++;
        if (coinsCollectedForHeal >= coinsForHeal)
        {
            // recompensa: cura completa + sonido
            life = maxLife;
            barraDeVida.CambiarVidaActual(life);
            healAudioSource?.PlayOneShot(healClip);
            coinsCollectedForHeal = 0;
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
                particulasCorrer.Play();  // Iniciar partículas cuando corra
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
                particulasCorrer.Stop();  // Detener partículas cuando no corra
            }

            if (WalkAudioSource.isPlaying)
            {
                WalkAudioSource.Stop();
            }
        }

        if (controller.IsDashing() && canDash && !isDashing)
        {
            StartCoroutine(Dash());
            particulasDash.Play();
            dashAudioSource.Play();
        }

        // Actualizar parámetros de animación
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

        if (!dashUnlocked) return; // Evita que el dash se use si no está desbloqueado

        if (controller.IsDashing() && canDash && !isDashing)
        {
            StartCoroutine(Dash());
            particulasDash.Play();
            dashAudioSource.Play();
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
        rb.velocity = new Vector2(dashSpeed * transform.localScale.x, 0);

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
        StartCoroutine(LostControl()); //Perder el control
        StartCoroutine(CollisionDesactive()); //Desactivar Colisiones
        Knockback(posicion);

        if (life <= 0)
        {
            life = 0;
            Dead();
            gamePlayCanvas.Onlose();
        }
    }
    private IEnumerator CollisionDesactive() //Knowback (Invensibilidad por un tiempito)
    {
        Physics2D.IgnoreLayerCollision(6, 8, true); //Desactiva las colisiones del player y los enemigos cuando ocurra el knockback
        yield return new WaitForSeconds(timeLostControl);
        Physics2D.IgnoreLayerCollision(6, 8, false);
    }
    private IEnumerator LostControl() //Knowback
    {
        canMove = false;
        yield return new WaitForSeconds(timeLostControl);
        canMove = true;
    }

    public void ResetPlayerCollisions()
    {
        Physics2D.IgnoreLayerCollision(6, 8, false);  // Asegúrate de que las colisiones se restauran
    }

    public void Curar(int cantidadCuracion)
    {
        life += cantidadCuracion;

        // Limitar la vida para que no sobrepase el máximo
        if (life > maxLife)
        {
            life = maxLife;
        }

        // Actualizar la barra de vida
        barraDeVida.CambiarVidaActual(life);
    }

    private void Dead()
    {
        Time.timeScale = 0;
        //Destroy(GetComponent<Player>(), 1);
    }


    public void RespawnAtCheckpoint()
    {
        if (checkpointManager != null)
        {
            // Desactiva momentáneamente el Rigidbody para evitar efectos residuales
            rb.velocity = Vector2.zero;
            transform.position = checkpointManager.GetCheckpointPosition();
            life = maxLife;
            barraDeVida.InicializarBarraDeVida(life);
            Debug.Log("Jugador respawneado en checkpoint: " + transform.position);
        }
    }

   

    // —— MÉTODO LINQ ORDERBY ——  
    public (Transform coin, float distance) GetClosestCoinInBox(float maxX, float maxY)
    {
        var allCoins = FindObjectsOfType<Coin>().Select(c => c.transform);
        var inBox = allCoins.Where(t =>
                          Mathf.Abs(t.position.x - transform.position.x) <= maxX &&
                          Mathf.Abs(t.position.y - transform.position.y) <= maxY);

        return inBox
               .Select(t => (coin: t, distance: Vector2.Distance(transform.position, t.position)))
               .OrderBy(x => x.distance)
               .FirstOrDefault();
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
        saveSystem.SaveDashState(dashUnlocked); // Guardar el estado del dash desbloqueado

        ButtonController controller = FindObjectOfType<ButtonController>();
        if (controller != null)
        {
            controller.SetDashButtonState(dashUnlocked); // Actualizar el botón de dash
        }
    }

    public float rangeEnemy = 20;
    private IEnumerator CheckNearbyEnemiesCoroutine()
    {
        
        while (true)
        {
            // Time-slicing: Verificar cada 2 segundos
            yield return new WaitForSeconds(2f);

            // LINQ Where (Grupo 1): Filtrar enemigos activos y cercanos
            var nearbyEnemies = FindObjectsOfType<Entity>()
                .Where(e => e.CompareTag("Enemy") &&
                            Vector2.Distance(transform.position, e.transform.position) < rangeEnemy)
                .ToList();

            // Tipo anónimo para UI
            var enemiesInfo = nearbyEnemies.Select(e => new
            {
                Name = e.gameObject.name,
                Distance = Vector2.Distance(transform.position, e.transform.position),
                Health = e.life
            }).ToList();

            // Actualizar UI (ejemplo)
            // En el método CheckNearbyEnemiesCoroutine:
            if (enemiesInfo.Any())
            {
                gamePlayCanvas.ShowEnemyAlert($"{enemiesInfo.Count} enemigos cerca!");
            }
            else
            {
                gamePlayCanvas.HideEnemyAlert();
            }
        }
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
}

