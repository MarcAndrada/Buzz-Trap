using UnityEngine;
public class YellowBee : Bee
{
    [Space, Header("Yellow Bee"), SerializeField]
    protected float chargeSpeed;

    [SerializeField]
    protected float randomOffset;

    [SerializeField]
    protected Vector2 randomTimeNoQueenMinMax;
    protected float timeToSpawnRandomDestination;
    protected float timeWaited;

    [HideInInspector]
    public Vector3 chargeDirection; 

    private void Start()
    {
        beeType = BeeType.YELLOW;

        timeToSpawnRandomDestination = Random.Range(randomTimeNoQueenMinMax.x, randomTimeNoQueenMinMax.y);
    }


    public override void QueenBehaviour()
    {
        if (!BeeManager.instance)
            return;

        switch (BeeManager.instance.yellowBeesState)
        {
            case BeeManager.YellowBeesStates.PLACING:
                MoveToDestiny(movementSpeed);
                Rotate(rotationSpeed);
                break;
            case BeeManager.YellowBeesStates.WAITING:
                MoveToDestiny(movementSpeed);
                Rotate(rotationSpeed);
                break;
            case BeeManager.YellowBeesStates.CHARGING:
                MoveToDestiny(chargeSpeed);
                Rotate(rotationSpeed);
                break;
            case BeeManager.YellowBeesStates.DRAG:
                break;
            default:
                break;
        }
    }
    public override void NoQueenBehaviour()
    {
        //Moverse Random
        WaitToGetRandomDestination();
        MoveToDestiny(movementSpeed);
        Rotate(rotationSpeed);
    }

    private void WaitToGetRandomDestination() 
    {
        timeWaited += Time.fixedDeltaTime;

        if (timeWaited < timeToSpawnRandomDestination)
            return;

        timeWaited = 0;
        destinationPos = transform.position + new Vector3(Random.Range(-randomOffset, randomOffset), 0, Random.Range(-randomOffset, randomOffset));
        rotationDestination = destinationPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(destinationPos, 0.1f);
        Gizmos.DrawLine(transform.position, destinationPos);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(rotationDestination, 0.1f);
        Gizmos.DrawLine(transform.position, rotationDestination);
    }
}
