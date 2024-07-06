using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference walkAction;
    [SerializeField] private InputActionReference rollAction;
    [SerializeField] private InputActionReference netAction;
    [SerializeField] private InputActionReference shieldAction;
    [SerializeField] private InputActionReference shotAction;

    [Header("MoveVariables")]
    [SerializeField] private float moveSpeed;

    [Header("RollVariables")]
    [SerializeField] private float rollTime;
    [SerializeField] private float forceRolling;

    [Header("NetVariables")]
    [SerializeField] private float netDistance;
    [SerializeField] private float netAreaEffect;
    [SerializeField] private LayerMask netAffectedMask;
    [SerializeField] private Animator netAnimator;

    [Header("ShieldVariables")]
    [SerializeField] private GameObject shield;

    [Header("ShootVariables")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletForce;

    private Rigidbody rb;
    private Animator animator;
    private EightDirectionAnimationController animationController;
    private Vector2 movementDirection;
    private Vector2 lastMovementDirection;
    private float rollCurrentTime;
    private float rollForce;

    private bool invulnarability;
    private bool shieldActive;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animationController = GetComponent<EightDirectionAnimationController>();
    }

    private void Start()
    {
        rollCurrentTime = 0;
        rollForce = 1;

        invulnarability = false;
        shieldActive = false;
        lastMovementDirection = Vector2.down;
        animationController.lookDirection = lastMovementDirection;

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
        shotAction.action.started += ShotAction;
    }

    private void OnDisable()
    {
        walkAction.action.started -= WalkAction;
        walkAction.action.performed -= WalkAction;
        walkAction.action.canceled -= WalkAction;

        rollAction.action.started -= RollAction;
        netAction.action.started -= NetAction;
        shieldAction.action.started -= ShieldAction;
        shotAction.action.started -= ShotAction;
    }
    #endregion

    private void WalkAction(InputAction.CallbackContext obj)
    {
        movementDirection = obj.ReadValue<Vector2>();
        animator.SetFloat("Movement", movementDirection.magnitude);
        if (movementDirection != Vector2.zero)
            lastMovementDirection = movementDirection;

        animationController.lookDirection = new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y);
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
        GameObject foundObject = CheckForwardObject();
        if (foundObject)
        {
            KillBee(foundObject);
        }
        netAnimator.enabled = true;
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

    private void ShotAction(InputAction.CallbackContext obj)
    {
        GameObject bulletCreated = Instantiate(bullet, transform.position, Quaternion.identity);
        bulletCreated.GetComponent<Rigidbody>().AddForce(lastMovementDirection.x * bulletForce, 0, lastMovementDirection.y * bulletForce);
    }

    private void Update()
    {
        IFrameRoll();
        Move();
    }

    private void Move()
    {
        rb.velocity = new Vector3(movementDirection.x, 0, movementDirection.y) * moveSpeed * rollForce;
    }
    private void IFrameRoll()
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
    }
    private GameObject CheckForwardObject()
    {
        Ray netRaycast = new Ray(transform.position, new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y));
        RaycastHit[] hits = Physics.SphereCastAll(netRaycast, netAreaEffect, netDistance, netAffectedMask);
        if (hits.Length > 0)
            return hits[0].collider.gameObject;

        return null;
    }

    private void KillBee(GameObject _foundObject)
    {
        Debug.Log("KillBee");
        StartCoroutine(BeeManager.instance.BeeCaught(_foundObject.GetComponent<Bee>()));
    }

    public void GetDamage()
    {
        //Quitar 1 de vida
        Debug.Log("Me hacen daño");
        //Empezar el tiempo de invulnerabilidad
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bee") && !invulnarability)
        {
            //Dañar al player
            GetDamage();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y) * netDistance, netAreaEffect);
    }
}
