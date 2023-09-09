using UnityEngine;
public class RootMotionTurn : StateMachineBehaviour
{
    private bool _tempRootMotion;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _tempRootMotion = animator.applyRootMotion;
        animator.applyRootMotion = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = _tempRootMotion;
    }
}
