using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetObjController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float distanceFromPlayer;

    private EightDirectionAnimationController playerAnimatorController;
    private EightDirectionAnimationController netAnimatorController;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Sprite topSprite;
    [SerializeField]
    private bool topFlip;
    [SerializeField]
    private Sprite topLeftSprite;
    [SerializeField]
    private bool topLeftFlip;
    [SerializeField]
    private Sprite leftSprite;
    [SerializeField] 
    private bool leftFlip;
    [SerializeField]
    private Sprite downLeftSprite;
    [SerializeField]
    private bool downLeftFlip;
    [SerializeField]
    private Sprite downSprite;
    [SerializeField]
    private bool downFlip;
    [SerializeField]
    private Sprite downRightSprite;
    [SerializeField]
    private bool downRightFlip;
    [SerializeField]
    private Sprite rightSprite;
    [SerializeField]
    private bool rightFlip;
    [SerializeField]
    private Sprite topRightSprite;
    [SerializeField]
    private bool topRightFlip;

    private void Awake()
    {
        playerAnimatorController = player.GetComponent<EightDirectionAnimationController>();
        netAnimatorController = GetComponent<EightDirectionAnimationController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        netAnimatorController.direction = playerAnimatorController.direction;
        netAnimatorController.ApplyCurrentAnimationController(netAnimatorController.direction);
        Vector3 relativePlayerPosition = GetPositionFromPlayerDirection(playerAnimatorController.direction);

        transform.position = player.transform.position + relativePlayerPosition * distanceFromPlayer;
    }

    public void DisableAnimator()
    {
        animator.enabled = false;
    }

    private Vector3 GetPositionFromPlayerDirection(EightDirectionAnimationController.Direction _direction)
    {
        Vector3 returnPos = Vector3.zero;

        switch (playerAnimatorController.direction)
        {
            case EightDirectionAnimationController.Direction.TOP:
                //Derecha
                returnPos = Vector3.right;
                spriteRenderer.sprite = topSprite;
                spriteRenderer.flipX = topFlip;
                break;
            case EightDirectionAnimationController.Direction.TOP_LEFT:
                //Arriba derecha
                returnPos = (Vector3.right + Vector3.forward).normalized;
                spriteRenderer.sprite = topLeftSprite;
                spriteRenderer.flipX = topLeftFlip;
                break;
            case EightDirectionAnimationController.Direction.TOP_RIGHT:
                //Abajo derecha
                returnPos = (Vector3.right + Vector3.back).normalized;
                spriteRenderer.sprite = topRightSprite;
                spriteRenderer.flipX = topRightFlip;
                break;
            case EightDirectionAnimationController.Direction.DOWN:
                //Izquierda
                returnPos = Vector3.left;
                spriteRenderer.sprite = downSprite;
                spriteRenderer.flipX = downFlip;
                break;
            case EightDirectionAnimationController.Direction.DOWN_LEFT:
                //Arriba izquierda
                returnPos = (Vector3.left + Vector3.forward).normalized;
                spriteRenderer.sprite = downLeftSprite;
                spriteRenderer.flipX = downLeftFlip;
                break;
            case EightDirectionAnimationController.Direction.DOWN_RIGHT:
                //Abajo Izquierda
                returnPos = (Vector3.left + Vector3.back).normalized;
                spriteRenderer.sprite = downRightSprite;
                spriteRenderer.flipX = downRightFlip;
                break;
            case EightDirectionAnimationController.Direction.LEFT:
                //Arriba
                returnPos = Vector3.forward;
                spriteRenderer.sprite = leftSprite;
                spriteRenderer.flipX = leftFlip;
                break;
            case EightDirectionAnimationController.Direction.RIGHT:
                //Abajo
                returnPos = Vector3.back;
                spriteRenderer.sprite = rightSprite;
                spriteRenderer.flipX = rightFlip;
                break;
            default:
                break;
        }

        return returnPos;        
    }
}
