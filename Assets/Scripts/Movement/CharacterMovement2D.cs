using Limbs;
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
        [SerializeField]
        float airControlEfficiency = 1f;

        private Vector2 _movement;
        private Vector2 _externalMovement;
        private Vector2 _projectedMovement;
        private Vector2 _projectedExternalMovement;
        private Vector2 _gravity;
        private Vector2 _groundNormal = Vector3.up;


        private Collider2D _collider2D;

        [SerializeField]
        bool grounded;
    
        Rigidbody2D _rigidbody2d;
        void Awake()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
        }

        void AirMovement(float h)
        {
            _gravity += Vector2.down * (9.81f * Time.deltaTime);

            Vector2 accelerationVector = new Vector2(h, 0) * (Time.deltaTime * speed * airControlEfficiency);
            _movement += accelerationVector;
            if (_movement.magnitude > speed)
                _movement = _movement.normalized * speed;
        }

        void Jump()
        {
            if (!grounded) return;
            _groundNormal = Vector3.up;
            _gravity = transform.up * (GetPlayerManager()?.GetLeg()?.GetJumpForce() ?? 1.5f);
            grounded = false;
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

            _externalMovement -= _externalMovement.normalized * (speed * Time.deltaTime) * 3.5f;

            float currentXSign = Mathf.Sign(_externalMovement.x);
            float currentYSign = Mathf.Sign(_externalMovement.y);

            if (currentXSign != previousXSign)
                _externalMovement.x = 0;

            if (currentYSign != previousYSign)
                _externalMovement.y = 0;
        }

        void Move(float h)
        {
            if (!grounded)
            {
                AirMovement(h);
            }
            else
            {
                GroundMovement(h);
                DecelerateExternalMovement();
            }

            _projectedMovement = Quaternion.FromToRotation(transform.up, _groundNormal) * _movement;
            _projectedExternalMovement = Quaternion.FromToRotation(transform.up, _groundNormal) * _externalMovement;
            _rigidbody2d.velocity = _projectedMovement + _gravity + _projectedExternalMovement;
        }

        // Update is called once per frame
        void Update()
        {
            grounded = GroundCast();

            if (Input.GetButtonDown("Jump"))
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
            _collider2D.enabled = false;
            _collider2D.enabled = true;
            _rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
            Transform myTransform = transform;
            myTransform.parent = null;
            myTransform.rotation = Quaternion.identity;
        }

        public void SetExternalMovement(Vector2 movement)
        {
            _externalMovement = movement;
        }
    }
}
