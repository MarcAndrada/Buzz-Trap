using UnityEngine;
public class BlackBee : Bee
{
    [Space, Header("Black Bee"), SerializeField]
    private float randomOffset;

    [SerializeField]
    private Vector2 randomTimeNoQueenMinMax;
    private float timeToSpawnRandomDestination;
    private float timeWaited;

    private void Start()
    {
        timeToSpawnRandomDestination = Random.Range(randomTimeNoQueenMinMax.x, randomTimeNoQueenMinMax.y);
    }


    protected override void MoveToDestiny(float _movementSpeed)
    {
        if (IsInDestiny())
            return;
        Vector3 direction = (destinationPos - transform.position).normalized;

        rb.velocity = direction * _movementSpeed;
    }
    public override void NoQueenBehaviour()
    {
        //Moverse Random
        WaitToGetRandomDestination();
        base.MoveToDestiny(movementSpeed);
        Rotate();
    }

    public override void QueenBehaviour()
    {
        MoveToDestiny(movementSpeed);
        Rotate();
    }

    private void WaitToGetRandomDestination()
    {
        timeWaited += Time.fixedDeltaTime;

        if (timeWaited < timeToSpawnRandomDestination)
            return;

        timeWaited = 0;
        timeToSpawnRandomDestination = Random.Range(randomTimeNoQueenMinMax.x, randomTimeNoQueenMinMax.y);
        destinationPos = transform.position + new Vector3(Random.Range(-randomOffset, randomOffset), 0, Random.Range(-randomOffset, randomOffset));
        rotationDestination = destinationPos;
    }
}
