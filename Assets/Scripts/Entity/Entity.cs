using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Configuración Remota")]
    public string enemyID; 

    [Header("Stats")]
    public int life;
    public int damageAttack;

    
    public void ConfigureEntity(string newName, int extraLife)
    {
        if (!string.IsNullOrEmpty(newName))
        {
            gameObject.name = newName;
        }
    }

    public void TakeDamage(int damage)
    {
        life -= damage;

        if (life <= 1.5f)
        {
            Death();
        }
    }

    public virtual void Death()
    {
        Destroy(gameObject);
    }
}