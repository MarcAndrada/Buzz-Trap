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

    [SerializeField] private float runForce;

    private GameObject parent;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bee"))
        {
            //Kill Bee
            Bee bee = collision.gameObject.GetComponent<Bee>();
            BeeManager.instance.CatchBee(bee);
            Instantiate(net, transform.position, Quaternion.identity);
            Destroy(gameObject);
            //Debug.DrawLine(transform.position, parent.transform.position, Color.yellow);
            //collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(transform.position.x - parent.transform.position.x, 0,
            //    transform.position.y - parent.transform.position.y) * runForce;

        }
    }

    }
        parent = _parent;
    {
    public void SetParent(GameObject _parent)
}
