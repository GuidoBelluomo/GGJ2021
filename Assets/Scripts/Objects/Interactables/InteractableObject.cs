using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Objects.Interactables
{
    [Serializable]
    public class InteractedEvent : UnityEvent {}
    public class InteractableObject : MonoBehaviour
    {
        private const float Cooldown = 5f;
        [SerializeField] private bool interactableWithKey;
        [SerializeField] private bool interactableWithLimb = true;
        private static readonly List<InteractableObject> Instances = new List<InteractableObject>();
        [SerializeField] InteractedEvent OnInteracted;
        private static bool _hashGenerated;
        private static int AnimInteracted;
        private bool _interacted = false;
        private Animator _animator;
        private float _lastInteraction = -10f;

        public virtual void Interact()
        {
            if (Time.unscaledTime < _lastInteraction + Cooldown) return;
            
            OnInteracted.Invoke();
            _interacted = !_interacted;
            
            if (_animator != null)
                _animator.SetBool(AnimInteracted, _interacted);

            _lastInteraction = Time.unscaledTime;
        }
        
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (!_hashGenerated)
            {
                AnimInteracted = Animator.StringToHash("Interacted");
                _hashGenerated = true;
            }
            
            Instances.Add(this);
        }
        
        private void OnDestroy()
        {
            Instances.Remove(this);
        }

        public bool IsInteractableWithInput()
        {
            return interactableWithKey;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!interactableWithLimb || !enabled) return;
            
            if (other.collider.gameObject.layer == 7 && other.collider.gameObject.transform.parent == null)
                Interact();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!interactableWithLimb || !enabled) return;
            
            if (other.gameObject.layer == 7 && other.gameObject.transform.parent == null)
                Interact();
        }

        public static InteractableObject GetClosestInteractable(Vector2 point, float maxDistance)
        {
            float curDistance = maxDistance + 1;
            InteractableObject closestObject = null;
            foreach (InteractableObject interactableObject in Instances)
            {
                float distance = Vector2.Distance((Vector2)interactableObject.transform.position, point);
                if (distance < curDistance && distance < maxDistance && interactableObject.enabled && interactableObject.IsInteractableWithInput())
                {
                    closestObject = interactableObject;
                    curDistance = distance;
                }
            }

            return closestObject;
        }
    }
}
