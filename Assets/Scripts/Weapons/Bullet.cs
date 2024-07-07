using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject net;

    private void Start()
    {
        Destroy(gameObject, 5);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bee"))
        {
            //Kill Bee
            Bee bee = collision.gameObject.GetComponent<Bee>();
            BeeManager.instance.CatchBee(bee);
            Instantiate(net, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

}
