using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bee : MonoBehaviour
{
    public enum BeeType { QUEEN, YELLOW, RED, BLACK }
    [Header("Base Bee"), SerializeField]
    public BeeType beeType;
    [SerializeField]
    protected float movementSpeed;
    [SerializeField]
    protected float rotationSpeed;
    protected Vector3 destinationPos;
    protected Vector3 rotationDestination;


    protected Rigidbody rb;

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    virtual protected void Start()
    {
        
    }


    protected void MoveToDestiny(float _movementSpeed)
    {
        Vector3 distance = destinationPos - transform.position;

        distance = distance.magnitude > 1 ? distance.normalized : distance;

        rb.AddForce(distance * _movementSpeed * Time.fixedDeltaTime, ForceMode.Force);
    }
    protected void Rotate(float _rotationSpeed)
    {
        if (Vector2.Distance(rotationDestination, transform.position) <= 0.5f)
            return;

        Vector3 targetDirection = rotationDestination - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.fixedDeltaTime);
    }

    public void GetDamage()
    {

    }

    public void SetDestination(Vector2 _destination)
    {
        destinationPos = new Vector3(_destination.x, _destination.y, transform.position.z);
    }
}
