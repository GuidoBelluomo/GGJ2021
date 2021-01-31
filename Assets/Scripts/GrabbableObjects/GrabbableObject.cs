using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace GrabbableObjects
{
    public class GrabbableObject : MonoBehaviour
    {
        private static readonly List<GrabbableObject> GrabbableObjects = new List<GrabbableObject>(); 
        [SerializeField]
        protected float grabRotation;
        private Rigidbody2D _rigidbody2D;

        public Rigidbody2D GetRigidBody2D()
        {
            return _rigidbody2D;
        }
        
        public void HandleGrab(Vector2 grabOffset)
        {
            Transform myTransform = transform;
            myTransform.localPosition = grabOffset;
            myTransform.localEulerAngles = new Vector3(0, 0, grabRotation);
        }

        private void Awake()
        {
            GrabbableObjects.Add(this);
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void OnDestroy()
        {
            GrabbableObjects.Remove(this);
        }

        public static GrabbableObject GetClosestGrabbableObject(Vector2 point, float maxDistance)
        {
            float curDistance = maxDistance + 1;
            GrabbableObject closestObject = null;
            foreach (GrabbableObject grabbableObject in GrabbableObjects)
            {
                float distance = Vector2.Distance((Vector2)grabbableObject.transform.position, point);
                if (distance < curDistance && distance < maxDistance)
                {
                    closestObject = grabbableObject;
                    curDistance = distance;
                }
            }

            return closestObject;
        }
    }
}
