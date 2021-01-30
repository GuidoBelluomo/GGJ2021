using System.Collections.Generic;
using Character;
using UnityEngine;

namespace Limbs
{
    public class BaseLimb : MonoBehaviour
    {
        private static readonly List<BaseLimb> AllLimbs = new List<BaseLimb>();
        
        private PlayerManager _playerManager;
        [SerializeField] private bool canBeLeg = true;
        [SerializeField] private bool canBeArm = true;
        [SerializeField] protected float yOffset = 0.8f;
        [SerializeField] protected float jumpForce = 4f;
        [SerializeField] protected float sacrificialJumpForce = 8f;
        [SerializeField] protected float sizeDownScale;
        
        private Rigidbody2D _rigidBody2D;
        private float _pickupColdown;

        public float GetJumpForce()
        {
            return jumpForce;
        }
        
        public float GetSacrificialJumpForce()
        {
            return sacrificialJumpForce;
        }
        
        public PlayerManager GetPlayerManager()
        {
            return _playerManager;
        }

        public void SetPlayerManager(PlayerManager playerManager)
        {
            this._playerManager = playerManager;
        }

        public bool CanBeLeg()
        {
            return canBeLeg;
        }

        public bool CanBeArm()
        {
            return canBeArm;
        }

        public float GetYOffset()
        {
            return yOffset;
        }
        
        public float GetSizeDownscale()
        {
            return sizeDownScale;
        }

        public enum Slot
        {
            Arm,
            Leg
        }

        public virtual void ArmPrimary()
        {

        }

        public virtual void LegPrimary()
        {

        }

        public virtual void OnSwapped()
        {
            
        }

        public virtual void Toss()
        {
            UnsetLimb();
            Rigidbody2D myRigidbody = GetRigidbody2D();
            myRigidbody.angularVelocity = Random.Range(-720f * GetPlayerManager().transform.localScale.x, -1080f * GetPlayerManager().transform.localScale.x);
            myRigidbody.velocity = GetPlayerManager().transform.right * (GetPlayerManager().transform.localScale.x * 15) + GetPlayerManager().transform.up * 5f;
        }
        
        public virtual void Sacrifice()
        {
            UnsetLimb();
            Rigidbody2D myRigidbody = GetRigidbody2D();
            float random = Random.Range(-5000f, 5000f);
            myRigidbody.angularVelocity = (Mathf.Max(3000f, Mathf.Abs(random)) * Mathf.Sign(random));
            myRigidbody.velocity = GetPlayerManager().GetRigidbody2D().velocity;
            myRigidbody.velocity += Vector2.down * 5f;
        }

        public void UnsetLimb()
        {
            GetPlayerManager().UnsetLimb(this);
        }

        public void Primary(Slot slot)
        {
            switch (slot)
            {
                case Slot.Arm:
                    ArmPrimary();
                    break;
                case Slot.Leg:
                    LegPrimary();
                    break;
            }
        }

        public virtual void ArmSecondary()
        {

        }

        public virtual void LegSecondary()
        {

        }

        public void Secondary(Slot slot)
        {
            switch (slot)
            {
                case Slot.Arm:
                    ArmSecondary();
                    break;
                case Slot.Leg:
                    LegSecondary();
                    break;
            }
        }

        void Awake()
        {
            AllLimbs.Add(this);
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }
        
        private void OnDestroy()
        {
            AllLimbs.Remove(this);
        }

        public Rigidbody2D GetRigidbody2D()
        {
            return _rigidBody2D;
        }
        
        public static BaseLimb GetClosestLimb(Vector2 point, float maxDistance)
        {
            float curDistance = maxDistance + 1;
            BaseLimb closestObject = null;
            foreach (BaseLimb limb in AllLimbs)
            {
                if (limb.transform.parent != null || limb._pickupColdown >= Time.time) continue;
                float distance = Vector2.Distance((Vector2)limb.transform.position, point);
                if (distance < curDistance)
                {
                    closestObject = limb;
                    curDistance = distance;
                }
            }

            return closestObject;
        }

        public void SetCooldown(float f)
        {
            _pickupColdown = Time.time + f;
        }
    }
}
