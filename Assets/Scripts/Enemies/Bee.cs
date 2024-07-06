using UnityEngine;

public abstract class Bee : MonoBehaviour
{
    public enum BeeType { QUEEN, YELLOW, RED, BLACK }
    [field: Header("Base Bee"), SerializeField]
    public BeeType beeType {  get; protected set; }
    [SerializeField]
    protected float movementSpeed;
    protected Vector3 destinationPos;
    protected Vector3 rotationDestination;
    [SerializeField]
    protected float minDistanceFromDestination;

    
    public Rigidbody rb {  get; protected set; }

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rotationDestination = transform.position + Vector3.back;
    }

    protected virtual void MoveToDestiny(float _movementSpeed)
    {
        Vector3 direction = destinationPos - transform.position;

        direction = direction.magnitude > 1f ? direction.normalized : direction;

        rb.velocity = direction * _movementSpeed;
    }
    protected void Rotate()
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

    protected virtual void OnDisable()
    {
        StartCoroutine(BeeManager.instance.BeeCaught(this));
        StartCoroutine(BeeManager.instance.BeeCaught(null));
    }


    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(rotationDestination, 0.1f);
        Gizmos.DrawLine(transform.position, rotationDestination);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(destinationPos, 0.1f);
        Gizmos.DrawLine(transform.position, destinationPos);
    }
}