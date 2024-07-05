using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject net;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bee"))
        {
            Destroy(gameObject);
            //Kill Bee
            Destroy(collision.gameObject);
            Instantiate(net, transform.position, Quaternion.identity);
        }
    }

    private void Update()
    {
        Destroy(gameObject, 5);
    }
}
