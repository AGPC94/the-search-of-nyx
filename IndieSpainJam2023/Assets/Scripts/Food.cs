using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    [SerializeField] float amountHeal;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance.Heal(amountHeal))
            {
                AudioManager.instance.Play("food");
                Destroy(gameObject);
            }
        }
    }
}
