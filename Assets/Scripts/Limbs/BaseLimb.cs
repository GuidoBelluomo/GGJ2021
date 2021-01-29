using Character;
using UnityEngine;

namespace Limbs
{
    public class BaseLimb : MonoBehaviour
    {
        private PlayerManager _playerManager;
        [SerializeField]
        private bool canBeLeg = false;
        [SerializeField]
        private bool canBeArm = true;
        [SerializeField]
        protected float yOffset = 0.8f;
        [SerializeField]
        protected float jumpForce = 4f;
        [SerializeField]
        protected float sacrificialJumpForce = 8f;
        
        private Rigidbody2D _rigidBody2D;

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
            myRigidbody.angularVelocity = Random.Range(25f, 50f);
            myRigidbody.velocity = GetPlayerManager().GetRigidbody2D().velocity * 2f;
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
            _rigidBody2D = GetComponent<Rigidbody2D>();
        }

        public Rigidbody2D GetRigidbody2D()
        {
            return _rigidBody2D;
        }
    }
}
