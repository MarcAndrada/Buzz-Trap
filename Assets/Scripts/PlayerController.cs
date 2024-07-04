using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference walkAction;
    [SerializeField] private InputActionReference rollAction;
    [SerializeField] private InputActionReference netAction;
    [SerializeField] private InputActionReference shieldAction;

    [Header("MoveVariables")]
    [SerializeField] private float moveSpeed;

    [Header("RollVariables")]
    [SerializeField] private float rollTime;
    [SerializeField] private float forceRolling;

    [Header("NetVariables")]
    [SerializeField] private float netDistance;
    [SerializeField] private LayerMask netAffectedMask;

    [Header("ShieldVariables")]
    [SerializeField] private GameObject shield;

    private Rigidbody rb;
    private Vector2 movementDirection;

    private float rollCurrentTime;
    private float rollForce;

    private bool invulnarability;
    private bool shieldActive;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        rollCurrentTime = 0;
        rollForce = 1;

        invulnarability = false;
        shieldActive = false;
    }

    #region input
    private void OnEnable()
    {
        walkAction.action.started += WalkAction;
        walkAction.action.performed += WalkAction;
        walkAction.action.canceled += WalkAction;

        rollAction.action.started += RollAction;
        netAction.action.started += NetAction;
        shieldAction.action.started += ShieldAction;
    }

    private void OnDisable()
    {
        walkAction.action.started -= WalkAction;
        walkAction.action.performed -= WalkAction;
        walkAction.action.canceled -= WalkAction;

        rollAction.action.started -= RollAction;
        netAction.action.started -= NetAction;
        shieldAction.action.started -= ShieldAction;
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

    private void ShieldAction(InputAction.CallbackContext obj)
    {
        if(!shieldActive)
        {
            shieldActive = true;
            GameObject shieldCreated = Instantiate(shield, transform.position, Quaternion.identity);
            shieldCreated.transform.SetParent(transform, true);
        }
    }

    private void Update()
    {
        if (invulnarability)
        {
            rollCurrentTime += Time.deltaTime;

            if (rollCurrentTime >= rollTime)
            {
                invulnarability = false;
                rollForce = 1;
                rollCurrentTime = 0;
            }
        }

        //if (shieldActive)
        //{
        //    shieldActive = false;
        //    Destroy(transform.GetChild(0).gameObject);
        //}
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
