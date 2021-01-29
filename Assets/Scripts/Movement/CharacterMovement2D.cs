using UnityEngine;

namespace Movement
{
    public class CharacterMovement2D : Movement2D
    {
        [SerializeField]
        float accelerationFactor = 1;
        [SerializeField]
        float groundedTolerance = 0.05f;
        [SerializeField]
        float decelerationFactor = 1;
        [SerializeField]
        float maxSpeed = 10;
        [SerializeField]
        float acceleration = 10;
        [SerializeField]
        float airControlEfficiency = 1f;
        [SerializeField]
        float jumpForce = 10;

        Vector2 _movement;
        Vector2 _projectedMovement;
        Vector2 _gravity;
        Vector2 _groundNormal = Vector3.up;


        Collider2D _collider2D;

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

            Vector2 accelerationVector = new Vector2(h, 0) * (Time.deltaTime * acceleration * accelerationFactor * airControlEfficiency);
            _movement += accelerationVector;
            if (_movement.magnitude > maxSpeed)
                _movement = _movement.normalized * maxSpeed;
        }

        void Jump()
        {
            if (!grounded) return;
            _groundNormal = Vector3.up;
            _gravity = transform.up * jumpForce;
            grounded = false;
        }

        void Accelerate(float h)
        {
            Vector2 accelerationVector = new Vector2(h, 0) * (Time.deltaTime * acceleration * accelerationFactor);
            _movement += accelerationVector;
            if (_movement.magnitude > maxSpeed)
                _movement = _movement.normalized * maxSpeed;
        }

        void Decelerate(float h)
        {
            Vector2 accelerationVector = new Vector2(h, 0) * (Time.deltaTime * acceleration * decelerationFactor);
            _movement += accelerationVector;
            if (_movement.magnitude > maxSpeed)
                _movement = _movement.normalized * maxSpeed;
        }

        void DecelerateToStop()
        {
            float previousXSign = Mathf.Sign(_movement.x);
            float previousYSign = Mathf.Sign(_movement.y);

            _movement -= _movement.normalized * (acceleration * decelerationFactor * Time.deltaTime);

            float currentXSign = Mathf.Sign(_movement.x);
            float currentYSign = Mathf.Sign(_movement.y);

            if (currentXSign != previousXSign)
                _movement.x = 0;

            if (currentYSign != previousYSign)
                _movement.x = 0;
        }

        void Move(float h)
        {
            if (!grounded)
            {
                AirMovement(h);
            }
            else
            {
                if (h != 0)
                {
                    if ((Mathf.Sign(h) + Mathf.Sign(_movement.x)) == 0)
                    {
                        Decelerate(h);
                    }
                    else
                    {
                        Accelerate(h);
                    }
                }
                else
                {
                    DecelerateToStop();
                }
            }

            _projectedMovement = Quaternion.FromToRotation(transform.up, _groundNormal) * _movement;
            _rigidbody2d.velocity = _projectedMovement + _gravity;
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

            ContactFilter2D filter = new ContactFilter2D();
            filter.layerMask = 1 << 6;
            RaycastHit2D[] results = new RaycastHit2D[1];
            int hits = _collider2D.Cast(Vector2.down, filter, results, groundedTolerance);
            if (hits > 0)
            {
                if (!grounded)
                {
                    transform.position = results[0].centroid;
                    _gravity = Vector2.zero;
                    _groundNormal = results[0].normal;
                }
                return true;
            }
            return false;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (GroundCast())
            {
                grounded = true;
            }

            if (ShouldKillSpeed())
            {
                _movement = Vector2.zero;
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (ShouldKillSpeed())
            {
                _movement = Vector2.zero;
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
            _rigidbody2d.gravityScale = 0;
            _rigidbody2d.freezeRotation = true;
            _rigidbody2d.sharedMaterial.friction = 0;
            transform.rotation = Quaternion.identity;
        }
    }
}
