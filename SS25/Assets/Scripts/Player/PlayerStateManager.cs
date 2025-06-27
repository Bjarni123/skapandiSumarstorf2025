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

        // return state.IsTag("AutoAttack") || state.IsTag("Roll");
        return !(state.IsTag("Idle") || state.IsTag("Walking") );
    }
}
