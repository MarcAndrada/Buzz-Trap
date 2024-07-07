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
    public bool stunned;
    [SerializeField]
    private float stunDuration;
    private float stunTimeWaited;
    

    public Rigidbody rb {  get; protected set; }
    protected EightDirectionAnimationController eightController;
    protected Animator animator;

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        eightController = GetComponent<EightDirectionAnimationController>();
        animator = GetComponent<Animator>();
        rotationDestination = transform.position + Vector3.back;
        UpdateAnimations();
    }

    protected virtual void MoveToDestiny(float _movementSpeed)
    {
        if (stunned)
            return;

        Vector3 direction = destinationPos - transform.position;

        direction = direction.magnitude > 1f ? direction.normalized : direction;

        rb.velocity = direction * _movementSpeed;
    }
    protected void WaitToStopStunned()
    {
        if (!stunned)
            return;

        stunTimeWaited += Time.fixedDeltaTime;

        if (stunTimeWaited >= stunDuration)
        {
            stunTimeWaited = 0;
            stunned = false;
        }

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

    public void UpdateAnimations()
    {
        if (eightController)
            eightController.lookDirection = (rotationDestination - transform.position).normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().GetDamage();
        }
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(destinationPos, 0.1f);
        Gizmos.DrawLine(transform.position, destinationPos);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(rotationDestination, 0.1f);
        Gizmos.DrawLine(transform.position, rotationDestination);

    }
}
