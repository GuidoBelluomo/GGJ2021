using UnityEngine;

namespace Limbs
{
    public class GrabberLimb : BaseLimb
    {
        [SerializeField]
        protected float grabRange = 0.2f;
        protected bool grabbing;
        private Rigidbody2D _grabbedBody;
        private FixedJoint2D _joint2D;

        private void GrabObject(Rigidbody2D body, Vector2 origin, Vector2 point)
        {
            _grabbedBody = body;
            _joint2D = GetPlayerManager().gameObject.AddComponent<FixedJoint2D>();
            _joint2D.connectedBody = _grabbedBody;
            _joint2D.connectedAnchor = point;
            _joint2D.anchor = transform.InverseTransformPoint(origin);
            grabbing = true;
        }
        
        private void ReleaseObject()
        {
            Destroy(_joint2D);
            _grabbedBody = null;
            grabbing = false;
        }
        
        public override void ArmPrimary()
        {
            base.ArmPrimary();
            if (!grabbing)
            {
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                var myTransform = transform;
                var position = myTransform.position;
                Vector3 grabOrigin = new Vector2(position.x, position.y + spriteRenderer.bounds.extents.y);
                RaycastHit2D hit = Physics2D.Raycast(grabOrigin, -myTransform.up, grabRange, 1 << 6 | 1 << 7);
                if (hit.rigidbody != null)
                {
                    GrabObject(hit.rigidbody, grabOrigin, hit.point);
                }
                else if (hit.collider != null)
                {
                    grabbing = true;
                }
            }
            else
            {
                ReleaseObject();
            }
        }
    }
}
