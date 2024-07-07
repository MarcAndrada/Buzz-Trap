using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject net;
    [SerializeField] private float runForce;

    private GameObject parent;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bee"))
        {
            Destroy(gameObject);
            //Kill Bee
            Destroy(collision.gameObject);
            Instantiate(net, transform.position, Quaternion.identity);
            //Debug.DrawLine(transform.position, parent.transform.position, Color.yellow);
            //collision.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(transform.position.x - parent.transform.position.x, 0,
            //    transform.position.y - parent.transform.position.y) * runForce;

        }
    }

    private void Update()
    {
        Destroy(gameObject, 5);
    }

    public void SetParent(GameObject _parent)
    {
        parent = _parent;
    }
}
