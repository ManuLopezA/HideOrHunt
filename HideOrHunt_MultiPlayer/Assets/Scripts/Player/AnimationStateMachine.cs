using Unity.Netcode;
using UnityEngine;

// THIS STATES MACHINE IS A "DEMO"
[RequireComponent(typeof(SpriteSelector))]
public class AnimationStateMachine : NetworkBehaviour
{
    private Animator animator;
    private float oldVelocity;
    [SerializeField] private GameObject sprite;

    private NetworkVariable<Direction> currentDirection = new(Direction.RIGHT);
    private NetworkVariable<SwitchMachineStates> currentState = new(SwitchMachineStates.IDLE);

    private enum SwitchMachineStates
    {
        IDLE, RUN
    };

    private enum Direction
    {
        LEFT, RIGHT
    };

    public Rigidbody rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentState.OnValueChanged += InitState;
    }

    private void Update()
    {
        UpdateState();
        if (!IsServer) return;
        if (rb.velocity == Vector3.zero)
        {
            currentState.Value = SwitchMachineStates.IDLE;
            return;
        }

        currentState.Value = SwitchMachineStates.RUN;

        if (rb.velocity.x > 0)
        {
            currentDirection.Value = Direction.RIGHT;
        }
        else if (rb.velocity.x < 0)
        {
            currentDirection.Value = Direction.LEFT;
        }
    }

    private void InitState(SwitchMachineStates prev, SwitchMachineStates curr)
    {
        switch (curr)
        {
            case SwitchMachineStates.IDLE:
                animator.Play("idle");
                break;
            case SwitchMachineStates.RUN:
                animator.Play("movement2");
                break;
        }
    }

    private void UpdateState()
    {
        switch (currentDirection.Value)
        {
            case Direction.LEFT:
                sprite.transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                break;
            case Direction.RIGHT:
                sprite.transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                break;
        }
    }

    public void ResetAnimator()
    {
        if (animator == null) return;
        animator.Rebind(); 
        animator.Update(0); 
    }
}
