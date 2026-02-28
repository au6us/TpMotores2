using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FondoMovimiento : MonoBehaviour
{
    [SerializeField] private Vector2 velocidadMovimiento; // Velocidad para controlar el parallax
    [SerializeField] private bool esSol = false; // Identificar si es el sol
    [SerializeField] private Vector2 tilingSol = new Vector2(1, 1); // Tiling personalizado para el sol
    private Vector2 offset;
    private Material material;
    private Rigidbody2D rb;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        // Si es el sol, ajustamos el tiling de la textura
        if (esSol)
        {
            material.mainTextureScale = tilingSol; // Ajustar el tiling si es el sol
            material.SetTextureOffset("_MainTex", Vector2.zero); // Asegurar que empiece en 0 el offset
        }
    }

    private void Update()
    {
        // Movimiento del fondo o el sol
        offset = (rb.velocity.x * 0.1f) * velocidadMovimiento * Time.deltaTime;

        // Si es el sol, aplicamos el offset de la textura con tiling ajustado
        if (esSol)
        {
            material.mainTextureOffset += offset;
        }
        else
        {
            material.mainTextureOffset += offset;
        }
    }
}
