using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBomb : MonoBehaviour
{
    [SerializeField] private float smokeVelocity;
    [SerializeField] private float height;
    private void Update()
    {
        if (transform.localScale.y < height)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(height, height, height), smokeVelocity);
        }

        Destroy(gameObject, 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bee"))
        {
            Debug.Log("si");
        }
    }
}
