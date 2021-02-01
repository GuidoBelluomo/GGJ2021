using Character;
using Objects.Pickups.Limbs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Movement
{
    public class CharacterMovement2D : Movement2D
    {
        [SerializeField]
        float groundedTolerance = 0.05f;
        [SerializeField]
        float speed = 5;
        //[SerializeField]
        //float airControlEfficiency = 1f;

        private Vector2 _movement;
        private Vector2 _externalMovement;
        private Vector2 _projectedMovement;
        private Vector2 _projectedExternalMovement;
        private Vector2 _gravity;
        private Vector2 _groundNormal = Vector3.up;
        [SerializeField] private float leglessJumpForce = 2.5f;


        private Collider2D _collider2D;

        [SerializeField]
        bool grounded;
    
        Rigidbody2D _rigidbody2d;
        void Awake()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
        }

        void AirMovement()
        {
            _gravity += Vector2.down * (9.81f * Time.deltaTime);
        }
        
        public void Jump()
        {
            if (!grounded) return;
            BaseLimb leg = GetPlayerManager()?.GetLeg();
            _groundNormal = Vector3.up;
            
            if (leg != null)
            {
                bool canBeLeg = leg.CanBeLeg();
                float jumpForce = canBeLeg ? leg.GetJumpForce() : leglessJumpForce;
                _gravity = transform.up * jumpForce;
                grounded = false;
            }
            else
            {
                _gravity = transform.up * leglessJumpForce;
                grounded = false;
            }
            
            GetPlayerManager().SetAnimationsBool(PlayerManager.AnimGrounded, false);
            GetPlayerManager().SetAnimationsBool(PlayerManager.AnimJumping, true);
        }

        void SacrificialJump()
        {
            if (!grounded || !GetPlayerManager().HasLeg()) return;
            _groundNormal = Vector3.up;
            _gravity = transform.up * (GetPlayerManager().GetLeg().GetSacrificialJumpForce());
            GetPlayerManager().GetLeg().Sacrifice();
            grounded = false;
            GetPlayerManager().SetAnimationsBool(PlayerManager.AnimGrounded, false);
            GetPlayerManager().SetAnimationsBool(PlayerManager.AnimHardJumping, true);
        }
        
        void GroundMovement(float h)
        {
            Vector2 accelerationVector = new Vector2(h * speed, 0) ;
            _movement = accelerationVector;
            if (_movement.magnitude > speed)
                _movement = _movement.normalized * speed;
        }
        
        void DecelerateExternalMovement()
        {
            if (_externalMovement == Vector2.zero) return;
            
            float previousXSign = Mathf.Sign(_externalMovement.x);
            float previousYSign = Mathf.Sign(_externalMovement.y);

            _externalMovement -= _externalMovement.normalized * (speed * Time.deltaTime * 3.5f);

            float currentXSign = Mathf.Sign(_externalMovement.x);
            float currentYSign = Mathf.Sign(_externalMovement.y);

            if (currentXSign != previousXSign)
                _externalMovement.x = 0;

            if (currentYSign != previousYSign)
                _externalMovement.y = 0;
        }

        void Move(float h)
        {
            GroundMovement(h);
            if (!grounded)
            {
                AirMovement();
            }
            else
            {
                DecelerateExternalMovement();
            }

            if (!Mathf.Approximately(h, 0))
            {
                transform.localScale = new Vector3(Mathf.Sign(h), 1, 1);
            }

            GetPlayerManager().SetAnimationsFloat(PlayerManager.AnimMoveSpeed, Mathf.Abs(_movement.x));

            _projectedMovement = Quaternion.FromToRotation(transform.up, _groundNormal) * _movement;
            _projectedExternalMovement = Quaternion.FromToRotation(transform.up, _groundNormal) * _externalMovement;
            _rigidbody2d.velocity = _projectedMovement + _gravity + _projectedExternalMovement;
        }

        // Update is called once per frame
        void Update()
        {
            grounded = GroundCast();
            GetPlayerManager().SetAnimationsBool(PlayerManager.AnimGrounded, grounded);
            if (grounded)
            {
                GetPlayerManager().SetAnimationsBool(PlayerManager.AnimJumping, false);
                GetPlayerManager().SetAnimationsBool(PlayerManager.AnimHardJumping, false);
            }
            else
            {
                _groundNormal = Vector2.zero;
            }

            if (Input.GetButtonDown("Jump"))
                if (Input.GetButton("Modifier"))
                    SacrificialJump();
                else
                    Jump();

            float h = Input.GetAxisRaw("Horizontal");
            Move(h);
        }

        bool GroundCast()
        {
            if (_gravity.y > 0)
                return false;

            RaycastHit2D[] results = new RaycastHit2D[10];
            int hits = _collider2D.Cast(Vector2.down, results, groundedTolerance);
            for (int i = 0; i < hits; i++)
            {
                RaycastHit2D result = results[i];
                if (result.transform.gameObject.layer != 7 && result.point.y < _collider2D.bounds.center.y)
                {
                    if (!grounded)
                    {
                        transform.position = result.centroid;
                    }
                    _gravity = Vector2.zero;
                    _groundNormal = result.normal;
                    _externalMovement.y = 0;
                    return true;
                }
            }
            return false;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (ShouldKillSpeed())
            {
                _movement = Vector2.zero;
                _externalMovement = Vector2.zero;
            }
        }

        bool ShouldKillSpeed()
        {
            Vector2 position = transform.position;
            Vector2 source = position;
            Vector2 dir = (_collider2D.bounds.extents.x + 0.05f) * _projectedMovement.normalized;

            RaycastHit2D hit = Physics2D.Linecast(source, source + dir, 1 << 6);

            source = position + (_collider2D.bounds.extents.y * _groundNormal * 0.2f);
            RaycastHit2D hit2 = Physics2D.Linecast(source, source + dir, 1 << 6);

            source = position - (_collider2D.bounds.extents.y * _groundNormal * 0.2f);
            RaycastHit2D hit3 = Physics2D.Linecast(source, source + dir, 1 << 6);

            return hit.collider != null || hit2.collider != null || hit3.collider != null;
        }

        private void OnEnable()
        {
            _movement = Vector2.zero;
            _gravity = Vector2.zero;
            _rigidbody2d.gravityScale = 0;
            _rigidbody2d.freezeRotation = true;
            _collider2D.sharedMaterial.friction = 0;
            _collider2D.enabled = false; // Resetting the collider to apply friction changes
            _collider2D.enabled = true;
            _rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
            Transform myTransform = transform;
            myTransform.parent = null;
            myTransform.rotation = Quaternion.identity;
            GetPlayerManager().GetKeepUpright().SetEnabled(true);
            GetPlayerManager().SetAnimationsBool(PlayerManager.AnimRollingMovement, false);
        }

        public void SetExternalMovement(Vector2 movement)
        {
            _externalMovement = movement;
        }

        public void AddExternalMovement(Vector2 movement)
        {
            _externalMovement += movement;
        }
    }
}
