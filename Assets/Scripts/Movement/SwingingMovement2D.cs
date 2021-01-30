using System;
using Character;
using Limbs;

namespace Movement
{
    using UnityEngine;
    public class SwingingMovement2D : Movement2D
    {
        private Rigidbody2D _rigidbody2d;
        private HingeJoint2D _hingeJoint2D;
        private Collider2D _collider2D;

        private void Awake()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
        }

        private void Update()
        {
            if (Mathf.Abs(_rigidbody2d.angularVelocity) < 100)
            {
                float h = Input.GetAxisRaw("Horizontal");
                _rigidbody2d.AddTorque(h * 0.25f);
            }

            if (Input.GetButtonDown("Jump"))
                Jump();
        }

        void Jump()
        {
            ((GrabberLimb)GetPlayerManager().GetArm()).ReleaseWorld(false, true);
        }
        
        private void OnEnable()
        {
            GetPlayerManager().SetAnimationsFloat(PlayerManager.AnimMoveSpeed, 0);
            _hingeJoint2D = GetPlayerManager().gameObject.AddComponent<HingeJoint2D>();
            JointAngleLimits2D jointLimits = new JointAngleLimits2D();
            jointLimits.min = 90;
            jointLimits.max = -90;
            _hingeJoint2D.limits = jointLimits;
            _hingeJoint2D.useLimits = true;
            _hingeJoint2D.anchor = transform.InverseTransformPoint(((GrabberLimb)GetPlayerManager().GetArm()).GetWorldGrabPoint());

            _rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody2d.velocity = Vector2.zero;
            _rigidbody2d.gravityScale = 1;
            _rigidbody2d.freezeRotation = false;
            _collider2D.sharedMaterial.friction = 0;
            _collider2D.enabled = false;
            _collider2D.enabled = true;
            GetPlayerManager().GetKeepUpright().SetEnabled(false);
        }

        private void OnDisable()
        {
            Destroy(_hingeJoint2D);
        }
    }
}