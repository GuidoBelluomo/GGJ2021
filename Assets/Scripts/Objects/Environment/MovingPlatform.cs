using System;
using UnityEditor;
using UnityEngine;

namespace Objects.Environment
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private Vector2 endOffset;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float roundTripTime = 5f;
        [SerializeField] private float restTime = 2f;
        [SerializeField] private bool platformEnabled;

        private Vector2 _startPosition;
        private Vector2 _colliderStartPosition;
        private BoxCollider2D _collider;
        private Rigidbody2D _rigidbody2D;
        private float _time = 0;
        private bool _returning = true;
        private bool _resting = true;

        public void SetEnabled(bool val)
        {
            platformEnabled = val;
        }

        void Awake()
        {
            _startPosition = transform.position;
            _collider = GetComponent<BoxCollider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _colliderStartPosition = _collider.bounds.center;

            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
            _rigidbody2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Update is called once per frame
        void OnDrawGizmos()
        {
            _collider = GetComponent<BoxCollider2D>();
            if (!Application.isPlaying)
            {
                Bounds bounds = _collider.bounds;
                _colliderStartPosition = bounds.center;
            }
            Vector2 startPoint = _colliderStartPosition;
            Vector2 endPoint = startPoint + endOffset;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(startPoint, endPoint);
            Gizmos.DrawWireCube(endPoint, _collider.bounds.size);
        }

        private void Update()
        {
            if (!platformEnabled) return;
            
            if (_resting)
            {
                _time += Time.deltaTime;
                if (_time >= restTime)
                {
                    _returning = !_returning;
                    _time = _returning ? 1 : 0;
                    _resting = false;
                }

                if (_resting) return;
            }
            
            if (_returning)
            {
                _time = Mathf.Clamp01(_time - Time.deltaTime / (roundTripTime / 2));
                if (_time == 0)
                    _resting = true;
            }
            else
            {
                _time = Mathf.Clamp01(_time + Time.deltaTime / (roundTripTime / 2));
                if (_time == 1)
                    _resting = true;
            }

            if (!_resting)
                _rigidbody2D.MovePosition(Vector3.Lerp(_startPosition, _startPosition + endOffset, curve.Evaluate(_time)));
            else
                _rigidbody2D.MovePosition(Vector3.Lerp(_startPosition, _startPosition + endOffset, _returning ? 0 : 1));
        }
    }
}
