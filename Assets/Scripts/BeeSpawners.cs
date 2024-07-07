using UnityEngine;

public class BeeSpawners : MonoBehaviour
{
    [SerializeField]
    private Vector2 minMaxTimeToSpawnBee;
    private float timeToSpawnNewBee;
    private float timeSpawnBeeWaited;

    private void Start()
    {
        timeToSpawnNewBee = GetRandomSpawnTime();
    }

    private void FixedUpdate()
    {
        WaitToSpawnNewBee();
    }

    private void WaitToSpawnNewBee()
    {
        timeSpawnBeeWaited += Time.fixedDeltaTime;

        if (timeSpawnBeeWaited >= timeToSpawnNewBee)
        {
            StartCoroutine(BeeManager.instance.SpawnNewBee(new Vector3(transform.position.x, 2.1f, transform.position.z)));
            timeSpawnBeeWaited = 0;
            timeToSpawnNewBee = GetRandomSpawnTime();
        }
    }

    private float GetRandomSpawnTime()
    {
        return Random.Range(minMaxTimeToSpawnBee.x, minMaxTimeToSpawnBee.y);
    }
}
