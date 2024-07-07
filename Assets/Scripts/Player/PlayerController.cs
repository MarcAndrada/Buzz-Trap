using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
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
    [SerializeField]
    private AudioClip[] footsteps;

    [Header("RollVariables")]
    [SerializeField] private float rollTime;
    [SerializeField] private float forceRolling;
    [SerializeField]
    private AudioClip rollClip;

    [Header("NetVariables")]
    [SerializeField] private float netDistance;
    [SerializeField] private float netAreaEffect;
    [SerializeField] private LayerMask netAffectedMask;
    [SerializeField] private Animator netAnimator;
    private float netCDWaited;
    [SerializeField] private float netCD;
    [SerializeField] private ParticleSystem netSlashParticles;
    [SerializeField]
    protected AudioClip netAttackClip;

    [Header("ShieldVariables")]
    [SerializeField] private GameObject shield;
    private GameObject existingShield;
    [SerializeField]
    private float shieldCD;
    private float shieldTimeWaited;
    [SerializeField]
    private UICooldownController shieldUiCd;
    [SerializeField]
    private AudioClip shieldActivationClip;
    [SerializeField]
    private AudioClip shieldDisableClip;

    [Header("ShootVariables")]
    [SerializeField] private GameObject bullet;
    [SerializeField]
    private float netShootCD;
    private float netShootTimeWaited;
    [SerializeField]
    private UICooldownController netUiCd;
    [SerializeField]
    private AudioClip netShootClip;

    [Space, Header("Health")]
    [SerializeField] private GameObject loseHealthParticles;
    [SerializeField] private float hitInvulnerabilityDuration;
    private float timeWaitedHit;
    [SerializeField]
    private int maxHealt;
    private int currentHealth;
    [SerializeField]
    private HealthUI healthUI;
    [SerializeField]
    private GameObject deadCanvas;
    [SerializeField]
    private GameObject sonCamera;

    [Header("SmokeVariables")]
    [SerializeField] private GameObject smoke;
    [SerializeField]
    private float smokeCD;
    private float smokeTimeWaited;
    [SerializeField]
    private UICooldownController smokeBombUiCd;
    [SerializeField]
    private AudioClip smokeClip;

    private Rigidbody rb;
    private Animator animator;
    private EightDirectionAnimationController animationController;
    private Vector2 movementDirection;
    private Vector2 lastMovementDirection;
    private float rollCurrentTime;
    private float rollForce;

    private bool invulnarability;
    private bool rolling;
    private bool shieldActive;
    private bool aiming;

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

        shieldUiCd.maxTime = shieldCD;
        shieldTimeWaited = shieldCD;
        smokeBombUiCd.maxTime = smokeCD;
        smokeTimeWaited = smokeCD;
        netUiCd.maxTime = netShootCD;
        netShootTimeWaited = netShootCD;

        currentHealth = maxHealt;

        healthUI.UpdateHearts(currentHealth);
    }
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

    #region input 
    
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
        if (!rolling)
        {
            rolling = true;
            rollForce = forceRolling;

            rb.velocity *= rollForce;

            animator.SetTrigger("Roll");
            AudioManager.instance.Play2dOneShotSound(rollClip, "Master", 1.2f);

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
        netSlashParticles.transform.rotation = Quaternion.LookRotation(Vector3.up, new Vector3(-lastMovementDirection.x, 0, -lastMovementDirection.y));
        netSlashParticles.Play();
        AudioManager.instance.Play2dOneShotSound(netAttackClip, "Master", 1.5f);
    }

    private void ShieldAction(InputAction.CallbackContext obj)
    {
        if (shieldCD > shieldTimeWaited)
            return;
        if (shieldActive)
            return;

        shieldActive = true;
        existingShield = Instantiate(shield, transform.position, Quaternion.identity);
        existingShield.transform.SetParent(transform, true);
        existingShield.transform.forward = Vector3.up;
        animator.SetTrigger("Spin");
        shieldTimeWaited = 0;
        AudioManager.instance.Play2dOneShotSound(shieldActivationClip, "Master", 1.5f);

    }

    private void ShotAction(InputAction.CallbackContext obj)
    {
        if (!aiming)
            return;

        aiming = false;

        Bullet bulletCreated = Instantiate(bullet, transform.position, Quaternion.identity).GetComponent<Bullet>();

        Vector2 shootDirection = lastMovementDirection.normalized;
        bulletCreated.direction = new Vector3(shootDirection.x, 0, shootDirection.y);
        netShootTimeWaited = 0;
        AudioManager.instance.Play2dOneShotSound(netShootClip, "Master", 1.5f);
    }

    private void AimAction(InputAction.CallbackContext obj)
    {

        if (netShootCD > netShootTimeWaited)
            return;
        aiming = true;
    }

    private void BombAction(InputAction.CallbackContext obj)
    {
        if (smokeCD > smokeTimeWaited)
            return;

        SmokeBomb bomb = Instantiate(smoke, transform.position, Quaternion.identity).GetComponent<SmokeBomb>();

        bomb.SetParent(gameObject);
        animator.SetTrigger("Spin");
        smokeTimeWaited = 0;
        AudioManager.instance.Play2dOneShotSound(smokeClip, "Master", 1.5f);
    }
    #endregion


    private void Update()
    {
        WaitMierdonesCD();
        WaitNetCD();
        IFrames();
        Move();
    }

    private void Move()
    {
        if (!aiming)
        {
            rb.velocity = new Vector3(movementDirection.x, 0, movementDirection.y) * moveSpeed * rollForce;
        }
        else
        {
            rb.velocity = Vector3.zero;
            animator.SetFloat("Movement", 0);
        }
    }
    private void IFrames()
    {
        if (invulnarability)
        {
            timeWaitedHit += Time.deltaTime;

            if(timeWaitedHit >= hitInvulnerabilityDuration)
            {
                timeWaitedHit = 0;
                invulnarability = false;
            }
        }

        if (rolling)
        {
            rollCurrentTime += Time.deltaTime;
            if (rollCurrentTime >= rollTime)
            {
                rollForce = 1;
                rollCurrentTime = 0;
                rolling = false;
            }

        }
    }

    private void WaitNetCD()
    {
        netCDWaited += Time.deltaTime;
    }
    private void WaitMierdonesCD()
    {
        shieldTimeWaited += Time.fixedDeltaTime;
        shieldUiCd.currentTime = shieldTimeWaited;

        smokeTimeWaited += Time.fixedDeltaTime;
        smokeBombUiCd.currentTime = smokeTimeWaited;

        netShootTimeWaited += Time.fixedDeltaTime;
        netUiCd.currentTime = netShootTimeWaited;
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
        if (invulnarability || rolling)
            return;
        if (shieldActive)
        {
            shieldActive = false;
            AudioManager.instance.Play2dOneShotSound(shieldDisableClip, "Master", 1.5f);
            Destroy(existingShield);
            return;
        }

        //Quitar 1 de vida
        currentHealth--;
        Instantiate(loseHealthParticles, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity);
        //Empezar el tiempo de invulnerabilidad
        invulnarability = true;
        healthUI.UpdateHearts(currentHealth);
        CheckIfDie();
    }

    private void CheckIfDie()
    {
        if (currentHealth > 0)
            return;

        sonCamera.transform.SetParent(null);
        deadCanvas.SetActive(true);
        gameObject.SetActive(false);
    }

    public bool AddHealth()
    {
        if (currentHealth == maxHealt)
            return false;

        currentHealth++;
        healthUI.UpdateHearts(currentHealth);
        return true;
    }

    public void PlayFootstep()
    {
        AudioManager.instance.PlayOneShotRandomSound2d(footsteps, "Master", 0.7f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(lastMovementDirection.x, 0, lastMovementDirection.y) * netDistance, netAreaEffect);
    }
}