using System;

namespace Movement
{
    using UnityEngine;
    public class SwingingMovement2D : Movement2D
    {
        private Rigidbody2D _rigidbody2d;
        // Start is called before the first frame update
        private void Awake()
        {
            _rigidbody2d = GetComponent<Rigidbody2D>();
        }
        
        private void OnEnable()
        {
            _rigidbody2d.gravityScale = 1;
            _rigidbody2d.freezeRotation = false;
            _rigidbody2d.sharedMaterial.friction = 0;
        }
    }
}