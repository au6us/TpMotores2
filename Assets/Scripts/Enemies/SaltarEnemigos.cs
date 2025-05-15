using System.Collections;
using UnityEngine;
using TMPro;

public class SaltarEnemigos : MonoBehaviour
{
    public int damage = 1;
    private Animator Animator;
    public GameObject floatingTextPrefab;

    [SerializeField] private float cantidadPuntos;
    [SerializeField] private Puntaje puntaje;

    [SerializeField] private GameObject efecto;

    //private bool hasBeenDeath = false;
    public AudioSource deathEnemySound;

    private void Start()
    {
        Animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D punto in other.contacts)
            {              

                if (punto.normal.y <= -0.9)
                {
                    Animator.SetTrigger("Golpe");
                    other.gameObject.GetComponent<Player>().Rebound();

                    if (deathEnemySound != null)
                    {
                        deathEnemySound.Play();
                    }

                    //hasBeenDeath = true;
                }

                else if (Mathf.Abs(punto.normal.x) > 0.5f)
                {
                    other.gameObject.GetComponent<Player>().TakeDamage(1, other.GetContact(0).normal); // Daño del knockback
                }
            }
        }
    }

    public void ShowFloatingText()
    {
        if (floatingTextPrefab)
        {
            var go = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            go.GetComponent<TextMeshPro>().text = cantidadPuntos.ToString();
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    public void Golpe()
    {
        ShowFloatingText();
        Instantiate(efecto, transform.position, transform.rotation);
        puntaje.SumarPuntos(cantidadPuntos);
        Destroy(gameObject);
    }
}