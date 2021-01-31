using System;
using UnityEngine;

namespace Objects.Environment
{
    public class TemporaryObstacle : MonoBehaviour
    {
        private Animator _animator;
        private static int AnimActivated;
        private static bool _hashGenerated;
        private bool _activated;

        public void DisableCollisions()
        {
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D collider2D in colliders)
            {
                collider2D.enabled = false;
            }
        }

        private void Awake()
        {
            if (!_hashGenerated)
            {
                AnimActivated = Animator.StringToHash("Activated");
                _hashGenerated = true;
            }

            _animator = GetComponent<Animator>();
        }

        public void Activate()
        {
            _activated = !_activated;
            _animator.SetBool(AnimActivated, _activated);
        }

        public void PlaySound(AudioClip clip)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.playOnAwake = false;
            source.Play();
            Destroy(source, clip.length);
        }
    }
}
