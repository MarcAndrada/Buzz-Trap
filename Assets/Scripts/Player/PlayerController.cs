using Unity.Burst.Intrinsics;
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
    [SerializeField] private InputActionReference bombAction;

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
    [SerializeField] private ParticleSystem netSlashParticles;

    [Header("ShieldVariables")]
    [SerializeField] private GameObject shield;
    private GameObject existingShield;
    [Header("ShootVariables")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletForce;

    [Space, Header("Health")]
    [SerializeField] private GameObject loseHealthParticles;

    [Header("SmokeVariables")]
    [SerializeField] private GameObject smoke;

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

    private Vector3 aimDirection;
    private Camera mainCamera;

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

        mainCamera = Camera.main;

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
        bombAction.action.started += BombAction;
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
        bombAction.action.started -= BombAction;
    }
    #endregion

    private void WalkAction(InputAction.CallbackContext obj)
    {
        if (!aiming)
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
        RaycastHit[] foundObjects = CheckForwardObjects();
        foreach (RaycastHit item in foundObjects)
            KillBee(item.collider.gameObject);
        netAnimator.enabled = true;
        netCDWaited = 0;
        netSlashParticles.gameObject.SetActive(true);
        netSlashParticles.transform.rotation = Quaternion.LookRotation(Vector3.up, new Vector3(-lastMovementDirection.x, 0 , -lastMovementDirection.y));
        netSlashParticles.Play();
    }

    private void ShieldAction(InputAction.CallbackContext obj)
    {
        if(!shieldActive)
        {
            shieldActive = true;
            existingShield = Instantiate(shield, transform.position, Quaternion.identity);
            existingShield.transform.SetParent(transform, true);
            existingShield.transform.forward = Vector3.up;
        }
    }

    private void ShotAction(InputAction.CallbackContext obj)
    {
        aiming = false;

        GameObject bulletCreated = Instantiate(bullet, transform.position, Quaternion.identity);

        Vector3 shootDirection = aimDirection.normalized;

        bulletCreated.GetComponent<Rigidbody>().AddForce(new Vector3(shootDirection.x, 0, shootDirection.z) * bulletForce, ForceMode.VelocityChange);
        bulletCreated.GetComponent<Bullet>().SetParent(gameObject);

        aimDirection = lastMovementDirection;
    }

    private void AimAction(InputAction.CallbackContext obj)
    {
        aiming = true;

        Vector3 mousePosition = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        if (ray.direction.y != 0)
        {
            float distance = -ray.origin.y / ray.direction.y;
            Vector3 hitPoint = ray.origin + ray.direction * distance;
            hitPoint.y = 0; 
            Debug.DrawLine(transform.position, hitPoint, Color.red);

            aimDirection = (hitPoint - transform.position).normalized;
        }
    }

    private void BombAction(InputAction.CallbackContext obj)
    {
        Instantiate(smoke, transform.position, Quaternion.identity);
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
        if(!aiming)
        {
            rb.velocity = new Vector3(movementDirection.x, 0, movementDirection.y) * moveSpeed * rollForce;
        }
        else
        {
            rb.velocity = Vector3.zero;
            animator.SetFloat("Movement", 0);
        }
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
    private RaycastHit[] CheckForwardObjects()
    {
        Ray netRaycast = new Ray(transform.position, new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y));
        RaycastHit[] hits = Physics.SphereCastAll(netRaycast, netAreaEffect, netDistance, netAffectedMask);
        return hits;
    }

    private void KillBee(GameObject _foundObject)
    {
        Debug.Log("KillBee");
        StartCoroutine(BeeManager.instance.BeeCaught(_foundObject.GetComponent<Bee>()));
    }

    public void GetDamage()
    {
        if (invulnarability)
            return;
        if(shieldActive)
        {
            shieldActive = false;
            Destroy(existingShield);
            return;
        }

        //Quitar 1 de vida
        Debug.Log("Me hacen da�o");
        Instantiate(loseHealthParticles, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
        //Empezar el tiempo de invulnerabilidad
    
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y) * netDistance, netAreaEffect);
    }
}
