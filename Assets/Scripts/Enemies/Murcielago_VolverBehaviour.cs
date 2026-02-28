using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Murcielago_VolverBehaviour : StateMachineBehaviour
{
    [SerializeField] private float velocidadMovimiento;
    private Vector3 puntoInicial;
    private Murcielago murcielago;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        murcielago = animator.gameObject.GetComponent<Murcielago>();
        puntoInicial = murcielago.puntoInicial;
    }

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, puntoInicial, velocidadMovimiento * Time.deltaTime);
        murcielago.Girar(puntoInicial);

        if(animator.transform.position == puntoInicial)
        {
            animator.SetTrigger("Llego");
        }
    }

    
}
