using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveGrass : MonoBehaviour
{
    private GameObject player;

    private Material grassMat;

    private void Start()
    {
        grassMat = GetComponent<Renderer>().material;
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        Vector3 trackerPos = player.transform.position;
        grassMat.SetVector("_Position", trackerPos);
    }
}
