using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class TelescopeManager : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Vector3 pos = transform.position;
        pos.x = screenBounds.x;
        pos.y = screenBounds.y;
        pos.z = 0;
        transform.position = pos;
    }

}