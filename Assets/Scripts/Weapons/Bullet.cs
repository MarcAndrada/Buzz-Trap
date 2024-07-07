using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject net;
    [SerializeField] private float bulletSpeed;
    [HideInInspector]
    public Vector3 direction;
    private Rigidbody rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        Destroy(gameObject, 8);
    }

    private void Update()
    {
        rb.velocity = direction * bulletSpeed;
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
