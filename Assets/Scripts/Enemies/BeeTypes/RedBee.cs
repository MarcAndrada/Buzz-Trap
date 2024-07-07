using UnityEngine;

public class RedBee : Bee
{
    [Space, Header("Red Bee"), SerializeField]
    private GameObject bulletPrefab;

    public float angle {  get; private set; }
    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float shootCD;
    private float timeShootWaited;

    [SerializeField]
    private float randomOffset;
    [SerializeField]
    protected AudioClip shootClip;

    private void Start()
    {
        angle = Random.Range(0f, 4f);
        timeShootWaited = Random.Range(0, shootCD / 2);
    }

    public override void NoQueenBehaviour()
    {
        timeShootWaited += Time.fixedDeltaTime;

        if (timeShootWaited >= shootCD)
        {
            //Generar punto random

            destinationPos = transform.position;
            rotationDestination = transform.position + new Vector3(Random.Range(-randomOffset, randomOffset), 0, Random.Range(-randomOffset, randomOffset));

            timeShootWaited = 0;
            Shoot();

        }

    }

    public override void QueenBehaviour()
    {
        angle += Time.fixedDeltaTime * rotationSpeed;

        timeShootWaited += Time.fixedDeltaTime;

        if (timeShootWaited >= shootCD)
        {
            timeShootWaited = 0;
            Shoot();

        }


        MoveToDestiny(movementSpeed);
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        bullet.GetComponent<RedBeeBulletController>().bulletDirection = (rotationDestination - transform.position).normalized;

        AudioManager.instance.Play3dOneShotSound(shootClip, "Master", 15f, transform.position, 1.5f);
    }


    
}
