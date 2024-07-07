using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference walkAction;
    [SerializeField] private InputActionReference rollAction;
    [SerializeField] private InputActionReference netAction;
    [SerializeField] private InputActionReference shieldAction;
    [SerializeField] private InputActionReference shotAction;
    [SerializeField] private InputActionReference aimAction;

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
    private float netCDWaited;
    [SerializeField] private float netCD;

    [Header("ShieldVariables")]
    [SerializeField] private GameObject shield;

    [Header("ShootVariables")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletForce;
    [SerializeField] private float degreeIncrease;

    private Rigidbody rb;
    private Animator animator;
    private EightDirectionAnimationController animationController;
    private Vector2 movementDirection;
    private Vector2 lastMovementDirection;
    private float rollCurrentTime;
    private float rollForce;

    private bool invulnarability;
    private bool shieldActive;
    private bool aiming;

    private float degree;

    private Vector2 aimDirection;

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
        degree = 0;

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
        shotAction.action.canceled += ShotAction;
        aimAction.action.started += AimAction;
    }

    private void OnDisable()
    {
        walkAction.action.started -= WalkAction;
        walkAction.action.performed -= WalkAction;
        walkAction.action.canceled -= WalkAction;

        rollAction.action.started -= RollAction;
        netAction.action.started -= NetAction;
        shieldAction.action.started -= ShieldAction;
        shotAction.action.canceled -= ShotAction;
        aimAction.action.started -= AimAction;
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

            animator.SetTrigger("Roll");
        }
    }

    private void NetAction(InputAction.CallbackContext obj)
    {
        if (netCDWaited < netCD)
            return;
        GameObject foundObject = CheckForwardObject();
        if (foundObject)
        {
            KillBee(foundObject);
        }
        netAnimator.enabled = true;
        netCDWaited = 0;
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
        aiming = false;
        GameObject bulletCreated = Instantiate(bullet, transform.position, Quaternion.identity);
        bulletCreated.GetComponent<Rigidbody>().AddForce(aimDirection.x * bulletForce, 0, aimDirection.y * bulletForce);
        aimDirection = lastMovementDirection;
        degree = 0;
    }

    private void AimAction(InputAction.CallbackContext obj)
    {
        aiming = true;
        aimDirection = lastMovementDirection;
        degree = 0;
    }

    private void Update()
    {
        WaitNetCD();
        IFrameRoll();
        Move();
        Aiming();

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
    
    private void WaitNetCD()
    {
        netCDWaited += Time.deltaTime;
    }

    private void Aiming()
    {
        if(aiming)
        {
            if (Input.GetAxis("Mouse Y") < 0)
            {
                degree -= degreeIncrease;
            }
            if (Input.GetAxis("Mouse Y") > 0)
            {
                degree += degreeIncrease;

            }
            Debug.Log(degree);
            aimDirection = Quaternion.Euler(0, 0, degree) * aimDirection;
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
            if(shieldActive)
            {
                Destroy(transform.GetChild(0));
                shieldActive = false;
            }
            else
            {
                //Dañar al player
                GetDamage();
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y) * netDistance, netAreaEffect);
    }
}
