using UnityEngine;
public class RootMotionTurn : StateMachineBehaviour
{
    public bool rootTo = true;
    private bool _tempRootMotion;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _tempRootMotion = animator.applyRootMotion;
        animator.applyRootMotion = rootTo;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = _tempRootMotion;
    }
}
