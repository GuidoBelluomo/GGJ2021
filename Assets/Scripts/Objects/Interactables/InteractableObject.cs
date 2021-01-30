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
        private static readonly List<InteractableObject> Instances = new List<InteractableObject>();
        public InteractedEvent OnInteracted;

        private void Awake()
        {
            Instances.Add(this);
        }
        
        private void OnDestroy()
        {
            Instances.Remove(this);
        }

        public static InteractableObject GetClosestInteractable(Vector2 point, float maxDistance)
        {
            float curDistance = maxDistance + 1;
            InteractableObject closestObject = null;
            foreach (InteractableObject interactableObject in Instances)
            {
                float distance = Vector2.Distance((Vector2)interactableObject.transform.position, point);
                if (distance < curDistance)
                {
                    closestObject = interactableObject;
                    curDistance = distance;
                }
            }

            return closestObject;

        }
    }
}
