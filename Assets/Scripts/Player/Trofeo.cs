using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trofeo : MonoBehaviour
{
    [SerializeField] private AudioSource winAudioSource;
    [SerializeField] public ParticleSystem sdpWin;
    [SerializeField] private GameplayManager gamePlayCanvas;
    [SerializeField] private Animator animatorTrophy;
    [SerializeField] private float delayWin = 3f;

    private bool hasPlayedSound = false;
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.GetComponent<Player>())
            {
                animatorTrophy.SetBool("Win", true);
                sdpWin.Play();

                if (!hasPlayedSound)
                {
                    winAudioSource.Play();
                    hasPlayedSound = true; 
                }

                if (levelManager != null)
                {
                    levelManager.CompleteLevel();
                    Debug.Log("Nivel completado, estrellas guardadas.");
                }
                else
                {
                    Debug.LogError("No se encontró LevelManager en la escena.");
                }

                StartCoroutine(ShowWinScreenAfterDelay());
            }
        }
    }

    private IEnumerator ShowWinScreenAfterDelay()
    {
        yield return new WaitForSeconds(delayWin);
        gamePlayCanvas.Onwin();
    }
}
