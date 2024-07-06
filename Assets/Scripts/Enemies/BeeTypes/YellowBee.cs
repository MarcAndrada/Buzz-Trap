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
                Rotate();
                break;
            case BeeManager.YellowBeesStates.WAITING:
                MoveToDestiny(movementSpeed);
                Rotate();
                break;
            case BeeManager.YellowBeesStates.CHARGING:
                MoveToDestiny(chargeSpeed);
                Rotate();
                break;
            case BeeManager.YellowBeesStates.DRAG:
                break;
            default:
                break;
        }
    }
    public void DefendQueenBehabiour()
    {
        MoveToDestiny(movementSpeed);
        Rotate();
    }
    public override void NoQueenBehaviour()
    {
        //Moverse Random
        WaitToGetRandomDestination();
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
