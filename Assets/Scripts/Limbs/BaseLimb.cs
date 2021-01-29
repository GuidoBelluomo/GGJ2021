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

        public virtual void OnMoved()
        {
            
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

        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
