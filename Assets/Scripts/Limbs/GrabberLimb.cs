using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberLimb : BaseLimb
{
    protected float grabRange = 0.2f;
    protected bool grabbing = false;
    public override void ArmPrimary()
    {
        base.ArmPrimary();
        if (!grabbing)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + spriteRenderer.bounds.extents.y), Vector2.up, grabRange, 1 << 6 | 1 << 7);
            if (hit.rigidbody != null)
            {
                Rigidbody2D rigidbody = hit.rigidbody;
                grabbing = true;
            }
            else if (hit.collider != null)
            {
                grabbing = true;
            }
        }
        else
        {
            grabbing = false;
        }
    }
}
