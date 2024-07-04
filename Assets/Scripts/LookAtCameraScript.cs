using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCameraScript : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        transform.forward = cam.transform.forward;
    }
}
