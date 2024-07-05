using UnityEngine;

public abstract class Bee : MonoBehaviour
{
    public enum BeeType { QUEEN, YELLOW, RED, BLACK }
    [field: Header("Base Bee"), SerializeField]
    public BeeType beeType {  get; protected set; }
    [SerializeField]
    protected float movementSpeed;
    [SerializeField]
    protected float rotationSpeed;
    protected Vector3 destinationPos;
    protected Vector3 rotationDestination;
    [SerializeField]
    protected float minDistanceFromDestination;

    
    public Rigidbody rb {  get; protected set; }

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void MoveToDestiny(float _movementSpeed)
    {
        Vector3 distance = destinationPos - transform.position;

        distance = distance.magnitude > 1f ? distance.normalized : distance;

        rb.velocity = distance * _movementSpeed * Time.fixedDeltaTime;
    }
    protected void Rotate(float _rotationSpeed)
    {
        //if (Vector3.Distance(rotationDestination, transform.position) <= 0.1f)
        //    return;

        //Vector3 targetDirection = rotationDestination - transform.position;
        //Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
    }

    public void GetDamage()
    {
        //Sumar +1 en el contador de abejas atrapadas

        //Hacer desaparecer la abeja
    }

    public void SetDestination(Vector3 _destination)
    {
        destinationPos = new Vector3(_destination.x, transform.position.y, _destination.z);
    }
    public void SetRotationDestiny(Vector3 _rotationDestination)
    {
        rotationDestination = new Vector3(_rotationDestination.x, transform.position.y, _rotationDestination.z);
    }
    public bool IsInDestiny()
    {
        return Vector3.Distance(transform.position, destinationPos) < minDistanceFromDestination;
    }

    public abstract void QueenBehaviour();
    public abstract void NoQueenBehaviour();
}
