using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioSource coinAudioSource; 
    [SerializeField] private ParticleSystem particulasDestello; 
    private bool hasBeenCollected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {        
        if (other.CompareTag("Player") && !hasBeenCollected)
        {            
            if (coinAudioSource != null)
            {
                coinAudioSource.Play();
            }

            if (particulasDestello != null)
            {
                particulasDestello.Play();
            }
            
            hasBeenCollected = true;
                        
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.CollectCoin(); // Añade una moneda al jugador
            }

            // Desactivar el renderer y el collider de la moneda para que no se pueda recolectar de nuevo
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;            
        }
    }
}