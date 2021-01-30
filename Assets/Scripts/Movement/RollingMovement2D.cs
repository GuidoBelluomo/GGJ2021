using Character;
using UnityEngine;

namespace Movement
{
    public class RollingMovement2D : Movement2D
    {
        [SerializeField]
        private float accelerationFactor = 1.5f;
        [SerializeField]
        private float decelerationFactor = 2.5f;
        [SerializeField]
        private float speedGain = 10;
        [SerializeField]
        private float maxSpeed = 10;
        [SerializeField]
        float groundedTolerance = 0.05f;

        private Rigidbody2D _rigidbody2d;
        private Collider2D _collider2D;
        void Awake()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Mathf.Abs(_rigidbody2d.angularVelocity) < maxSpeed)
            {
                float h = -Input.GetAxisRaw("Horizontal");
                if (Mathf.Sign(h) + Mathf.Sign(_rigidbody2d.angularVelocity) != 0)
                    _rigidbody2d.angularVelocity += h * speedGain * accelerationFactor * Time.deltaTime;
                else
                    _rigidbody2d.angularVelocity += h * speedGain * decelerationFactor * Time.deltaTime;
                
                GetPlayerManager().SetAnimationsFloat(PlayerManager.AnimMoveSpeed, Mathf.Abs(_rigidbody2d.angularVelocity));
            }

            if (Input.GetButtonDown("Jump"))
                Jump();
        }
        
        void Jump()
        {
            RaycastHit2D[] results = new RaycastHit2D[10];
            int hits = _collider2D.Cast(Vector2.down, results, groundedTolerance);
            for (int i = 0; i < hits; i++)
            {
                RaycastHit2D result = results[i];
                if (result.transform.gameObject.layer != 7 && result.point.y < _collider2D.bounds.center.y)
                {
                    _rigidbody2d.AddForce(Vector3.up * 1.5f, ForceMode2D.Impulse);
                }
            }
        }
        
        private void OnEnable()
        {
            transform.parent = null;
            _rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody2d.gravityScale = 1;
            _rigidbody2d.freezeRotation = false;
            _collider2D.sharedMaterial.friction = 1;
            _collider2D.enabled = false;
            _collider2D.enabled = true;
            GetPlayerManager().GetKeepUpright().SetEnabled(true);
            GetPlayerManager().SetAnimationsBool(PlayerManager.AnimRolling, true);
        }
    }
}
