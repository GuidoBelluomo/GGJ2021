using Character;
using UnityEngine;

namespace AnimationBehaviours
{
    public class SetJumpToFalse : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(PlayerManager.AnimJumping, false);
            animator.SetBool(PlayerManager.AnimHardJumping, false);
        }
    }
}