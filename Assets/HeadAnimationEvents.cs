using System.Collections;
using System.Collections.Generic;
using Character;
using Movement;
using UnityEngine;

public class HeadAnimationEvents : MonoBehaviour
{
    public void Jump()
    {
        transform.parent.GetComponent<CharacterMovement2D>().Jump();
    }
}
