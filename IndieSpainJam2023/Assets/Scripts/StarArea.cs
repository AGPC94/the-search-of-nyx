using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarArea : MonoBehaviour
{
    [SerializeField] GameObject starSky;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.starSkyCur = starSky;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}