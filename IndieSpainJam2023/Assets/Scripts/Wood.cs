using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance.AddWood(1))
            {
                AudioManager.instance.Play("wood");
                Destroy(gameObject);
            }
        }
    }
}
