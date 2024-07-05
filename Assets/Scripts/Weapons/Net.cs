using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour
{
    [SerializeField] private float netVelocity;
    [SerializeField] private float height;

    private bool netPrepared;

    private void Start()
    {
        netPrepared = false;
    }

    private void Update()
    {
        if(transform.localScale.y < height)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(height, height, height), netVelocity);
            netPrepared = true;
        }

        if(netPrepared) 
        { 
            Destroy(gameObject, 2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bee") && netPrepared)
        {
            //Kill Bee
        }
    }
}
