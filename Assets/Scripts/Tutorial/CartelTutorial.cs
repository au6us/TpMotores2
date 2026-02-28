using UnityEngine;

public class CartelTutorial : MonoBehaviour
{
    [Header("Contenido de ESTE Tutorial")]
    [Tooltip("Escribí el nombre exacto del video con su extensión, ej: tutorial_salto.mp4")]
    [SerializeField] private string nombreArchivoVideo;
    [TextArea(3, 5)]
    [SerializeField] private string textoTutorial;

    [Header("Configuración de Cooldown")]
    [SerializeField] private float cooldownTiempo = 4f;
    private float proximoTiempoPermitido = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (Time.unscaledTime >= proximoTiempoPermitido)
            {
                PanelTutorial.Instance.AbrirTutorial(nombreArchivoVideo, textoTutorial, this);
            }
        }
    }

    public void IniciarCooldown()
    {
        proximoTiempoPermitido = Time.unscaledTime + cooldownTiempo;
    }
}