using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Murcielago_SeguirBehaviour : StateMachineBehaviour
{
    [SerializeField] private float velocidadMovimiento;

    [SerializeField] private float tiempoBase;

    private float tiempoSeguir;

    private Transform player;

    private Murcielago murcielago;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        tiempoSeguir = tiempoBase;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        murcielago = animator.gameObject.GetComponent<Murcielago>();
    }

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, player.position, velocidadMovimiento * Time.deltaTime);
        murcielago.Girar(player.position);
        tiempoSeguir -= Time.deltaTime;
        if(tiempoSeguir <= 0)
        {
            animator.SetTrigger("Volver");
        }
    }

    
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    
}
