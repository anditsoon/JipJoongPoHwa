using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera allyCamera;

    void Update()
    {
        if(gameObject.name == "AllyHP")
        {
            transform.forward = allyCamera.transform.forward;
        }
        else
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
