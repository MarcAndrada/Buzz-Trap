using UnityEngine;

public class EightDirectionAnimationController : MonoBehaviour
{
    public Vector2 lookDirection;
    public enum Direction { TOP, TOP_LEFT, TOP_RIGHT, DOWN, DOWN_LEFT, DOWN_RIGHT, LEFT, RIGHT }

    [SerializeField]
    private AnimatorOverrideController topAnimator;
    [SerializeField]
    private AnimatorOverrideController topLeftAnimator;
    [SerializeField]
    private AnimatorOverrideController leftAnimator;
    [SerializeField]
    private AnimatorOverrideController downLeftAnimator;
    [SerializeField]
    private AnimatorOverrideController downAnimator;
    [SerializeField]
    private AnimatorOverrideController downRightAnimator;
    [SerializeField]
    private AnimatorOverrideController rightAnimator;
    [SerializeField]
    private AnimatorOverrideController topRightAnimator;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        Direction currentDirection = CheckLookingDirection();
        ApplyCurrentAnimationController(currentDirection);
    }

    private Direction CheckLookingDirection()
    {
        Vector3 lookDirection = new Vector3(this.lookDirection.x, transform.position.y, this.lookDirection.y);

        float topDot = Vector3.Dot(lookDirection, Vector3.forward);
        float topLeftDot = Vector3.Dot(lookDirection, (Vector3.forward + Vector3.left).normalized);
        float leftDot = Vector3.Dot(lookDirection, Vector3.left);
        float downLeftDot = Vector3.Dot(lookDirection, (Vector3.back + Vector3.left).normalized);
        float downDot = Vector3.Dot(lookDirection, Vector3.back);
        float downRightDot = Vector3.Dot(lookDirection, (Vector3.back + Vector3.right).normalized);
        float rightDot = Vector3.Dot(lookDirection, Vector3.right);
        float topRightDot = Vector3.Dot(lookDirection, (Vector3.forward + Vector3.right).normalized);

        if (topDot > topLeftDot && topDot > topRightDot)
            return Direction.TOP;
        else if(topLeftDot > topDot && topLeftDot> leftDot)
            return Direction.TOP_LEFT;
        else if (leftDot > topLeftDot && leftDot > downLeftDot)
            return Direction.LEFT;
        else if (downLeftDot > leftDot && downLeftDot > downDot)
            return Direction.DOWN_LEFT;
        else if (downDot > downLeftDot && downDot > downRightDot)
            return Direction.DOWN;
        else if (downRightDot > downDot && downRightDot > rightDot)
            return Direction.DOWN_RIGHT;
        else if (rightDot > downRightDot && rightDot > topRightDot)
            return Direction.RIGHT;
        else
            return Direction.TOP_RIGHT;
    }

    private void ApplyCurrentAnimationController(Direction _currentDirection)
    {
        switch (_currentDirection)
        {
            case Direction.TOP:
                animator.runtimeAnimatorController = topAnimator;
                break;
            case Direction.TOP_LEFT:
                animator.runtimeAnimatorController = topLeftAnimator;
                break;
            case Direction.TOP_RIGHT:
                animator.runtimeAnimatorController = topRightAnimator;
                break;
            case Direction.DOWN:
                animator.runtimeAnimatorController = downAnimator;
                break;
            case Direction.DOWN_LEFT:
                animator.runtimeAnimatorController = downLeftAnimator;
                break;
            case Direction.DOWN_RIGHT:
                animator.runtimeAnimatorController = downRightAnimator;
                break;
            case Direction.LEFT:
                animator.runtimeAnimatorController = leftAnimator;
                break;
            case Direction.RIGHT:
                animator.runtimeAnimatorController = rightAnimator;
                break;
            default:
                break;
        }
    }
}
