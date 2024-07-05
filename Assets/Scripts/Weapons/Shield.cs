using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private float shieldVelocity;
    [SerializeField] private float height;
    private void Update()
    {
        if (transform.localScale.y < height)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(height, height, height), shieldVelocity);
        }
    }
}
