using UnityEngine;
using UnityEngine.Video;
using TMPro;
using System.Collections;

public class PanelTutorial : MonoBehaviour
{
    public static PanelTutorial Instance;

    [Header("Referencias de UI Tutorial")]
    [SerializeField] private GameObject panelPrincipal;
    [SerializeField] private GameObject videoObjeto;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private TextMeshProUGUI textoTutorial;
    [SerializeField] private GameObject botonCerrar;

    [Header("Referencias de Gameplay")]
    [SerializeField] private GameObject[] canvasGameplay;

    [Header("Animación")]
    [SerializeField] private float tiempoAnimacion = 0.25f;

    private CartelTutorial cartelActual;
    private Vector3 escalaOriginalVideo;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        videoPlayer.isLooping = true;
        videoPlayer.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;

        if (videoObjeto != null)
        {
            escalaOriginalVideo = videoObjeto.transform.localScale;
        }

        panelPrincipal.SetActive(false);
        botonCerrar.SetActive(false);
        textoTutorial.gameObject.SetActive(false);

        videoPlayer.loopPointReached += OnVideoLoopComplete;
    }

    public void AbrirTutorial(string nombreVideo, string texto, CartelTutorial cartel)
    {
        cartelActual = cartel;
        textoTutorial.text = texto;

        ButtonController btnController = FindObjectOfType<ButtonController>();
        if (btnController != null)
        {
            btnController.stopMovement();
        }

        foreach (GameObject canvas in canvasGameplay)
        {
            if (canvas != null) canvas.SetActive(false);
        }

        Player jugador = FindObjectOfType<Player>();
        if (jugador != null) jugador.SilenciarAudio();

        StartCoroutine(RutinaPrepararYMostrar(nombreVideo));
    }

    private IEnumerator RutinaPrepararYMostrar(string nombreVideo)
    {
        // --- EL ARREGLO ESTÁ ACÁ ---
        // Congelamos el tiempo AL INSTANTE, antes de que Unity se ponga a cargar el video.
        // Así los enemigos y el cronómetro se clavan en el lugar al milisegundo que tocás el cartel.
        Time.timeScale = 0.0001f;

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Application.streamingAssetsPath + "/" + nombreVideo;

        videoPlayer.Prepare();

        // Mientras el video carga por detrás, el juego ya está en pausa, así que no perdés nada de tiempo.
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();
        StartCoroutine(AnimacionAparecer());
    }

    private IEnumerator AnimacionAparecer()
    {
        panelPrincipal.SetActive(true);
        textoTutorial.gameObject.SetActive(true);
        botonCerrar.SetActive(false);

        videoObjeto.transform.localScale = Vector3.zero;
        videoObjeto.SetActive(true);

        float tiempo = 0f;
        while (tiempo < tiempoAnimacion)
        {
            tiempo += Time.unscaledDeltaTime;
            float progreso = tiempo / tiempoAnimacion;
            videoObjeto.transform.localScale = Vector3.Lerp(Vector3.zero, escalaOriginalVideo, progreso);
            yield return null;
        }

        videoObjeto.transform.localScale = escalaOriginalVideo;
    }

    private void OnVideoLoopComplete(VideoPlayer vp)
    {
        if (!botonCerrar.activeSelf)
        {
            botonCerrar.SetActive(true);
        }
    }

    public void CerrarTutorial()
    {
        StartCoroutine(AnimacionDesaparecer());
    }

    private IEnumerator AnimacionDesaparecer()
    {
        botonCerrar.SetActive(false);
        textoTutorial.gameObject.SetActive(false);

        float tiempo = 0f;
        while (tiempo < tiempoAnimacion)
        {
            tiempo += Time.unscaledDeltaTime;
            float progreso = tiempo / tiempoAnimacion;
            videoObjeto.transform.localScale = Vector3.Lerp(escalaOriginalVideo, Vector3.zero, progreso);
            yield return null;
        }

        videoObjeto.SetActive(false);
        panelPrincipal.SetActive(false);

        foreach (GameObject canvas in canvasGameplay)
        {
            if (canvas != null) canvas.SetActive(true);
        }

        if (cartelActual != null)
        {
            cartelActual.IniciarCooldown();
        }

        Time.timeScale = 1f;

        yield return new WaitForSecondsRealtime(0.1f);

        videoPlayer.Pause();
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoLoopComplete;
        }
    }
}