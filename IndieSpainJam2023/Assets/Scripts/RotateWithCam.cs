using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithCam : MonoBehaviour
{
    void FixedUpdate()
    {
        Vector3 q = transform.eulerAngles;
        q.z = Camera.main.transform.eulerAngles.z;
        transform.eulerAngles = q;
    }
}
