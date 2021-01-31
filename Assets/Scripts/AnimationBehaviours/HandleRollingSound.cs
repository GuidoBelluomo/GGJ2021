using Character;
using UnityEngine;

namespace AnimationBehaviours
{
    public class HandleRollingSound : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.transform.parent.GetComponent<PlayerManager>().StartRollingSound();
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.transform.parent.GetComponent<PlayerManager>().EndRollingSound();
        }
    }
}
