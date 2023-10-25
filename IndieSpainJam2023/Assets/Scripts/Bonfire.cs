using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Bonfire : MonoBehaviour
{
    //¿Poner luz?

    [SerializeField] Color colorBurnOut;
    ParticleSystem particle;


    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.Play("bonfire");

        particle = transform.GetComponentInChildren<ParticleSystem>();
        particle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (DayNightSystem2D.instance.IsDay())
        {
            //Cambiar a animacion apagada
            AudioManager.instance.Stop("bonfire");
            particle.Stop();

            foreach (var item in transform.GetComponentsInChildren<SpriteRenderer>())
            {
                item.color = colorBurnOut;
            }

            GetComponent<Light2D>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
    }
}
