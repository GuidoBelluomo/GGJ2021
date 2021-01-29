using UnityEngine;

namespace Movement
{
    public class RollingMovement2D : Movement2D
    {
        [SerializeField]
        float accelerationFactor = 1.5f;
        [SerializeField]
        float decelerationFactor = 2.5f;
        [SerializeField]
        float speedGain = 10;
        [SerializeField]
        float maxSpeed = 10;

        Rigidbody2D _rigidbody2d;
        void Awake()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
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
            }
        }
        
        private void OnEnable()
        {
            _rigidbody2d.gravityScale = 1;
            _rigidbody2d.freezeRotation = false;
            _rigidbody2d.sharedMaterial.friction = 1;
        }
    }
}
