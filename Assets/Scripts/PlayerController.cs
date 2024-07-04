using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference walkAction;
    [SerializeField] private InputActionReference rollAction;
    [SerializeField] private InputActionReference netAction;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rollTime;
    [SerializeField] private float forceRolling;
    [SerializeField] private float netDistance;

    [SerializeField] private LayerMask netAffectedMask;

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

    #region input
    private void OnEnable()
    {
        walkAction.action.started += WalkAction;
        walkAction.action.performed += WalkAction;
        walkAction.action.canceled += WalkAction;

        rollAction.action.started += RollAction;
        netAction.action.started += NetAction;
    }

    private void OnDisable()
    {
        walkAction.action.started -= WalkAction;
        walkAction.action.performed -= WalkAction;
        walkAction.action.canceled -= WalkAction;

        rollAction.action.started -= RollAction;
        netAction.action.started -= NetAction;
    }
    #endregion

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

    private void NetAction(InputAction.CallbackContext obj)
    {
        if(CheckForwardObject())
        {
            KillBee();
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

    private bool CheckForwardObject()
    {
        Ray netRaycast = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(netRaycast, netDistance, netAffectedMask))
        {
            return true;
        }
        return false;
    }

    private void KillBee()
    {
        Debug.Log("KillBee");
    }
}
