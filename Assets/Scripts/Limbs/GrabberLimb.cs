using Character;
using GrabbableObjects;
using Movement;
using UnityEngine;

namespace Limbs
{
    public class GrabberLimb : BaseLimb
    {
        [SerializeField]
        protected Vector2 grabOffset;
        [SerializeField]
        protected float grabRange = 0.5f;
        [SerializeField]
        protected float pickupRange = 0.75f;
        [SerializeField]
        protected float throwMultiplier = 1.5f;

        private bool _grabbingWorld;
        private bool _grabbingObject;
        private Rigidbody2D _grabbedBody;
        private Vector2 _grabPointOffset;
        
        public override void OnSwapped()
        {
            ReleaseObject();
            ReleaseWorld();
        }
        
        public override void Toss()
        {
            if (_grabbingWorld)
            {
                UnsetLimb();
                GetPlayerManager().UpdateMovementType(true);
                ReleaseWorld(false);
                GetRigidbody2D().bodyType = RigidbodyType2D.Static;
            }
            else
            {
                base.Toss();
            }
        }
        
        private void GrabObject(GrabbableObject grabbableObject)
        {
            _grabbedBody = grabbableObject.GetRigidBody2D();
            _grabbedBody.bodyType = RigidbodyType2D.Kinematic;
            _grabbedBody.transform.parent = transform;
            grabbableObject.HandleGrab(grabOffset);
            _grabbingObject = true;
        }
        
        private void ReleaseObject()
        {
            _grabbedBody.bodyType = RigidbodyType2D.Dynamic;
            _grabbedBody.velocity = GetPlayerManager().GetRigidbody2D().velocity * throwMultiplier;
            _grabbedBody.transform.parent = null;
            _grabbedBody = null;
            _grabbingObject = false;
        }
        
        public override void ArmPrimary()
        {
            base.ArmPrimary();
            if (_grabbingWorld) return;
            if (!_grabbingObject)
            {
                GrabbableObject grabbableObject = GrabbableObject.GetClosestGrabbableObject(GetPlayerManager().GetBottomPosition(), pickupRange);
                if (grabbableObject != null)
                {
                    GrabObject(grabbableObject);
                }
            }
            else
            {
                ReleaseObject();
            }
        }

        private void GrabWorld(Vector2 origin, RaycastHit2D hit)
        {
            _grabPointOffset = hit.point - origin;
            PlayerManager playerManager = GetPlayerManager();
            Rigidbody2D playerRigidBody = playerManager.GetRigidbody2D();
            Vector2 rigidbodyVelocity = playerRigidBody.velocity / 5;
            playerManager.SetMovementType(PlayerManager.MovementType.Swinging);
            playerRigidBody.AddForceAtPosition(rigidbodyVelocity, playerManager.GetBottomPosition(), ForceMode2D.Impulse);
            GetPlayerManager().transform.position += (Vector3)_grabPointOffset;
            _grabbingWorld = true;
        }

        public void ReleaseWorld(bool adjustOffset = true, bool violent = false)
        {
            if (!_grabbingWorld) return;

            float angularVelocity = GetPlayerManager().GetRigidbody2D().angularVelocity;
            
            if (!violent)
            {
                GetPlayerManager().UpdateMovementType(true);
                GetPlayerManager().GetCharacterMovement2D().SetExternalMovement((Vector2.right * (angularVelocity / 22.5f)));
            }
            else
            {
                Toss();
                CharacterMovement2D characterMovement2D = GetPlayerManager().GetCharacterMovement2D();
                RollingMovement2D rollingMovement2D = GetPlayerManager().GetRollingMovement2D();
                
                if (characterMovement2D.enabled)
                    characterMovement2D.SetExternalMovement((Vector2.right * (angularVelocity / 15)) + (Vector2.up * (angularVelocity / 50)));

                if (rollingMovement2D.enabled)
                    GetPlayerManager().GetRigidbody2D().velocity = (Vector2.right * (angularVelocity / 15)) +
                                                                   (Vector2.up * (angularVelocity / 50));
            }
            
            _grabbingWorld = false;
            if (adjustOffset)
            {
                GetPlayerManager().transform.position -= (Vector3)_grabPointOffset;
            }
        }

        public override void ArmSecondary()
        {
            base.ArmSecondary();
            if (_grabbingObject) return;
            if (!_grabbingWorld)
            {
                var myTransform = transform;
                Vector3 grabOrigin = transform.TransformPoint(grabOffset);
                RaycastHit2D hit = Physics2D.Raycast(grabOrigin, myTransform.up, grabRange, 1 << 6);
    
                if (hit.collider != null)
                {
                    GrabWorld(grabOrigin, hit);
                }
            }
            else
            {
                ReleaseWorld(false);
            }
        }
        
        public Vector3 GetWorldGrabPoint()
        {
            return transform.TransformPoint(grabOffset);
        }
        
        public void OnDrawGizmos()
        {
            Transform myTransform = transform;
            Vector2 localOffset = myTransform.TransformPoint(grabOffset);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(new Vector3(localOffset.x, localOffset.y, myTransform.position.z), 0.05f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(localOffset, (Vector3)localOffset + transform.up * grabRange);
        }
    }
}
