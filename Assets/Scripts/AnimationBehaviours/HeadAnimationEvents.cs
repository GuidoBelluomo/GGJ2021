using Movement;
using UnityEngine;

namespace AnimationBehaviours
{
    public class HeadAnimationEvents : MonoBehaviour
    {
        public void Jump()
        {
            transform.parent.GetComponent<CharacterMovement2D>().Jump();
        }
    }
}
