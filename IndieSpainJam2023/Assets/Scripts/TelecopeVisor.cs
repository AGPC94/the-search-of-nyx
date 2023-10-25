using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelecopeVisor : MonoBehaviour
{
    [SerializeField] float speed = 50f;
    [SerializeField] float limitX = 50f;
    [SerializeField] float limitY = 50f;
    Vector2 screenBounds;
    float objectWidth;
    float objectHeight;

    bool canMove = true;

    bool isMapOpen;

    // Use this for initialization
    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x; //extents = size of width / 2
        objectHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y; //extents = size of height / 2
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove == false)
            return;

        Vector2 move;
        move.x = Input.GetAxisRaw("Horizontal") * speed;
        move.y = Input.GetAxisRaw("Vertical") * speed;
        transform.Translate(move * Time.unscaledDeltaTime);

        Vector2 viewPos = transform.localPosition;

        //viewPos.x = Mathf.Clamp(viewPos.x, -screenBounds.x + objectWidth, screenBounds.x - objectWidth);
        //viewPos.y = Mathf.Clamp(viewPos.y, -screenBounds.y + objectHeight, screenBounds.y - objectHeight);

        viewPos.x = Mathf.Clamp(viewPos.x, -limitX, limitX);
        viewPos.y = Mathf.Clamp(viewPos.y, -limitY, limitY);


        transform.localPosition = viewPos;


        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isMapOpen)
            {
                isMapOpen = false;
                GameManager.instance.HideMap();
            }
            else
            {
                isMapOpen = true;
                GameManager.instance.ShowMap();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TargetStar"))
        {
            if (canMove)
            {
                AudioManager.instance.Play("findStar");
                //GameManager.instance.ShowMap();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("TargetStar"))
        {
            if (canMove)
            {
                GameManager.instance.HideMap();
            }
        }

    }


    void OnEnable()
    {
        canMove = true;
        transform.localPosition = Vector3.zero;
    }
}

/* 


Mapa estelar
    segunda camara que enfoque las star areas con un icono de la cxonstelacion
    mostarlo en el render texture
*/