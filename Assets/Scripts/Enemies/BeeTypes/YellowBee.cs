using UnityEngine;
public class YellowBee : Bee
{
    [Space, Header("Yellow Bee"), SerializeField]
    protected float chargeSpeed;

    [SerializeField]
    protected float randomOffset;
    [SerializeField]
    protected float timeToSpawnRandomDestination;
    protected float timeWaited;

    private void Start()
    {
        beeType = BeeType.YELLOW;
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
        WaitToGetARandomDestination();
        MoveToDestiny(movementSpeed);
        Rotate(rotationSpeed);
    }

    private void WaitToGetARandomDestination() 
    {
        timeWaited += Time.fixedDeltaTime;

        if (timeWaited < timeToSpawnRandomDestination)
            return;

        timeWaited = 0;
        destinationPos += new Vector3(Random.Range(-randomOffset, randomOffset), 0, Random.Range(-randomOffset, randomOffset));
        rotationDestination = destinationPos;
    }
}
