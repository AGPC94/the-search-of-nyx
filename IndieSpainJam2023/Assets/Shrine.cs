using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour
{
    [SerializeField] bool isON = false;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!isON)
            {
                isON = true;
                //Particula
                //Sonido
                GameManager.instance.nShrines++;
            }
        }
    }
}
