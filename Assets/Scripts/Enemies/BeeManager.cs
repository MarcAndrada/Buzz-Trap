using AYellowpaper.SerializedCollections.Editor.Data;
using System.Collections.Generic;
using UnityEngine;

public class BeeManager : MonoBehaviour
{
    public static BeeManager instance;


    public bool queenAlive;
    private bool closeToQueen;
    public enum YellowBeesStates { NONE, PLACING, WAITING, CHARGING, DRAG }

    [SerializeField]
    private GameObject player;
    [SerializeField]
    private List<Bee> bees;

    private int queenBeeId;

    [Space, Header("Yellow Bees"), SerializeField]
    private int beesPerRow;
    [SerializeField]
    private float beesPlayerOffset;
    [SerializeField]
    private float beesOffset;
    [SerializeField]
    private float yellowBeesPlacingDuration;
    [SerializeField]
    private float yellowBeesWaitingDuration;
    [SerializeField]
    private float yellowBeesChargeDuration;
    [SerializeField]
    private float yellowBeesDragDuration;
    private float yellowBeesTimeWaited;
    [field: SerializeField]
    public YellowBeesStates yellowBeesState { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }

        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < bees.Count; i++)
        {
            if (bees[i].beeType == Bee.BeeType.QUEEN)
            {
                queenBeeId = i;
                queenAlive = true;
            }
        }
        yellowBeesState = YellowBeesStates.PLACING;
        yellowBeesTimeWaited = 0;
    }

    private void FixedUpdate()
    {
        BeesUpdate();
    }

    private void BeesUpdate()
    {
        int currentYellowBeeID = 0;
        yellowBeesTimeWaited += Time.fixedDeltaTime;

        foreach (Bee bee in bees)
        {
            switch (bee.beeType)
            {
                case Bee.BeeType.QUEEN:
                    QueenUpdate((QueenBee)bee);
                    break;
                case Bee.BeeType.YELLOW:
                    YellowBeesUpdate((YellowBee)bee, currentYellowBeeID);
                    currentYellowBeeID++;
                    break;
                case Bee.BeeType.RED:
                    RedBeesUpdate((RedBee)bee);
                    break;
                case Bee.BeeType.BLACK:
                    BlackBeesUpdate((BlackBee)bee);
                    break;
            }


        }

        switch (yellowBeesState)
        {
            case YellowBeesStates.NONE:

                break;
            case YellowBeesStates.PLACING:
                if (yellowBeesTimeWaited >= yellowBeesPlacingDuration)
                {
                    yellowBeesState = YellowBeesStates.WAITING;
                    yellowBeesTimeWaited = 0;
                }
                break;
            case YellowBeesStates.WAITING:
                if (yellowBeesTimeWaited >= yellowBeesWaitingDuration)
                {
                    yellowBeesState = YellowBeesStates.CHARGING;
                    yellowBeesTimeWaited = 0;
                }
                break;
            case YellowBeesStates.CHARGING:
                if (yellowBeesTimeWaited >= yellowBeesChargeDuration)
                {
                    yellowBeesState = YellowBeesStates.DRAG;
                    yellowBeesTimeWaited = 0;
                }
                break;
            case YellowBeesStates.DRAG:
                if (yellowBeesTimeWaited >= yellowBeesChargeDuration)
                {
                    yellowBeesState = YellowBeesStates.PLACING;
                    yellowBeesTimeWaited = 0;
                }
                break;
            default:
                break;
        }

    }

    private void QueenUpdate(QueenBee _bee)
    {
        if (!queenAlive)
        {
            _bee.NoQueenBehaviour();
            return;
        }
    }

    private void YellowBeesUpdate(YellowBee _bee, int _beeId)
    {

        if (!queenAlive)
        {
            _bee.NoQueenBehaviour();
            return;
        }

        switch (yellowBeesState)
        {
            case YellowBeesStates.PLACING:

                Vector3 destinyPos = CalculateNewColumnRowPosition(_beeId);

                //Cambiar la posicion de destino
                _bee.SetDestination(destinyPos);
                //Comprobar si la distancia entre su posicion de destino y la actual
                if (_bee.IsInDestiny())
                {
                    //Si es mas grande que X el punto de rotacion sera el de destino
                    _bee.SetRotationDestiny(destinyPos);
                }
                else
                {
                    //Si no sera el Player
                    _bee.SetRotationDestiny(player.transform.position);
                }              

                break;
            case YellowBeesStates.WAITING:
                
                destinyPos = CalculateNewColumnRowPosition(_beeId);
                //Cambiar la posicion de destino
                _bee.SetDestination(destinyPos);

                //El punto de rotacion es el player
                _bee.SetRotationDestiny(player.transform.position);

                break;
            case YellowBeesStates.CHARGING:
                //Calcular una nueva posicion y rotacion hacia el forward de la abeja
                Vector3 newDestiny = _bee.transform.right * 10;
                _bee.SetDestination(newDestiny);
                _bee.SetRotationDestiny(newDestiny);

                //Esperar para cambiar al drag
                if (yellowBeesTimeWaited >= yellowBeesChargeDuration)
                    _bee.rb.drag /= 2;
                break;
            case YellowBeesStates.DRAG:                
                //Esperar para volver a colocarse
                //Al acabar el tiempo de drag volver el drag a su origen
                if (yellowBeesTimeWaited >= yellowBeesChargeDuration)
                    _bee.rb.drag *= 2;
                break;
            default:
                break;
        }

        _bee.QueenBehaviour();
    }
    private Vector3 CalculateNewColumnRowPosition(int _beeId)
    {
        int directionID = _beeId % 4;
        Vector3 direction;
        Vector3 axis;
        if (directionID == 0)
        {
            direction = Vector3.forward;
            axis = Vector3.right;
        }
        else if (directionID == 1)
        {
            direction = Vector3.right;
            axis = Vector3.back;

        }
        else if (directionID == 2)
        {
            direction = Vector3.left;
            axis = Vector3.forward;
        }
        else
        {
            direction = Vector3.back;
            axis = Vector3.left;
        }

        int columnId = Mathf.FloorToInt(_beeId / beesPerRow) % 4;
        int rowId = Mathf.FloorToInt(_beeId / (beesPerRow * 4));

        //Calcular su posicion de destino
        Vector3 starterMiddlePos = direction * beesPlayerOffset;
        Vector3 starterPos = starterMiddlePos + (direction * beesOffset * rowId) - (axis * (beesOffset / beesPerRow));
        Vector3 destinyPos = starterPos + (axis * (beesOffset / beesPerRow)) * columnId;

        Vector3 resultPos = player.transform.position + destinyPos;
        return resultPos;  
    }
    

    private void RedBeesUpdate(RedBee _bee)
    {
        if (!queenAlive)
        {
            _bee.NoQueenBehaviour();
            return;
        }
    }

    private void BlackBeesUpdate(BlackBee _bee)
    {
        if (!queenAlive)
        {
            _bee.NoQueenBehaviour();
            return;
        }
    }



    private void OnDrawGizmos()
    {
        int _beeId = 0;

        foreach (Bee bee in bees)
        {
            switch (bee.beeType)
            {
                case Bee.BeeType.YELLOW:
                    Gizmos.color = Color.yellow;
                    YellowBee _bee = (YellowBee)bee;

                    Vector3 destinyPos = CalculateNewColumnRowPosition(_beeId);
                    //Cambiar la posicion de destino
                    Gizmos.DrawSphere(destinyPos, 0.1f);
                    Gizmos.DrawLine(_bee.transform.position, destinyPos);
                    _beeId++;
                    break;
                default: 
                    break;
            }


        }

        //Vector3 direction = Vector3.forward;
        //Vector3 axis = Vector3.right;
        //Vector3 starterPos;
        //int rowId;
        //Vector3 pos;


        //Gizmos.color = Color.blue;
        //rowId = 0;
        //starterPos = player.transform.position + (direction * (beesPlayerOffset + (beesOffset * rowId)) - (axis * ((beesPerRow / 2) * beesOffset)));
        //pos = starterPos + axis * 0;
        //Gizmos.DrawSphere(pos, 0.1f);
        //Debug.Log("Pos de id 0 = " + pos);

        //Gizmos.color = Color.magenta;
        //pos = starterPos + axis * 4;
        //Gizmos.DrawSphere(pos, 0.1f);
        //Debug.Log("Pos de id 4 = " + pos);

        //Gizmos.color = Color.white;
        //pos = starterPos + axis * 8;
        //Gizmos.DrawSphere(starterPos, 0.1f);
        //Debug.Log("Pos de id 8 = " + pos);


    }
}
