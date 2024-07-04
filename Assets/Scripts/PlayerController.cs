using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference walkAction;
    [SerializeField] private InputActionReference rollAction;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rollTime;
    [SerializeField] private float forceRolling;

    private Rigidbody rb;
    private Vector2 movementDirection;

    private float rollCurrentTime;
    private float rollForce;
    private bool invulnarability;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rollCurrentTime = 0;
        rollForce = 1;
    }

    private void OnEnable()
    {
        walkAction.action.started += WalkAction;
        walkAction.action.performed += WalkAction;
        walkAction.action.canceled += WalkAction;

        rollAction.action.started += RollAction;
    }

    private void OnDisable()
    {
        walkAction.action.started -= WalkAction;
        walkAction.action.performed -= WalkAction;
        walkAction.action.canceled -= WalkAction;

        rollAction.action.started -= RollAction;
    }

    private void WalkAction(InputAction.CallbackContext obj)
    {
        movementDirection = obj.ReadValue<Vector2>();

        rb.velocity = new Vector3(movementDirection.x, 0, movementDirection.y) * moveSpeed * rollForce;
    }

    private void RollAction(InputAction.CallbackContext obj)
    {
        if(!invulnarability)
        {
            invulnarability = true;
            rollForce = forceRolling;

            rb.velocity *= rollForce;
        }
    }

    private void Update()
    {
        if(invulnarability)
        {

            rollCurrentTime += Time.deltaTime;

            if(rollCurrentTime >= rollTime)
            {
                invulnarability = false;
                rollForce = 1;
                rollCurrentTime = 0;
            }
        }
    }
}
