using UnityEngine;

public class RedBeeBulletController : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed;
    [HideInInspector]
    public Vector3 bulletDirection;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, 5);
    }
    private void FixedUpdate()
    {
        rb.velocity = bulletDirection * bulletSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().GetDamage(); //Hacer daño
            Destroy(gameObject);
        }
    }
}
