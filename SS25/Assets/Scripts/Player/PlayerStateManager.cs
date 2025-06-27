using UnityEngine;
    
public class PlayerStateManager : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public bool IsPerformingAction()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

        return !(state.IsTag("Idle") || state.IsTag("Walking") );
    }

    public bool IsPerformingAttack()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        return state.IsTag("AutoAttack");
    }

    public bool IsPerformingRoll()
    {
        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        return state.IsTag("Rolling");
    }
}
