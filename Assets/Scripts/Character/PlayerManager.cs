using System;
using Limbs;
using Movement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class PlayerManager : MonoBehaviour
    {
        public enum MovementType
        {
            Rolling,
            Walking,
            Swinging
        }

        private MovementType _movementType = MovementType.Rolling;

        public const int ArmSlot = (int) BaseLimb.Slot.Arm;
        public const int LegSlot = (int) BaseLimb.Slot.Leg;

        private BaseLimb[] limbs = new BaseLimb[2] {null, null};

        public delegate void LimbSwap(BaseLimb[] limbs);

        public event LimbSwap OnLimbSwap;

        [FormerlySerializedAs("arm")] [SerializeField]
        GameObject testArm;

        [FormerlySerializedAs("leg")] [SerializeField]
        GameObject testLeg;

        Collider2D _collider2D;
        Rigidbody2D _rigidbody2D;

        private CharacterMovement2D _characterMovement2D;
        private RollingMovement2D _rollingMovement2D;
        private SwingingMovement2D _swingingMovement2D;

        public MovementType GetMovementType()
        {
            return _movementType;
        }

        public void SetMovementType(MovementType movementType)
        {
            _movementType = movementType;

            switch (movementType)
            {
                case MovementType.Rolling:
                    _rollingMovement2D.enabled = true;
                    _characterMovement2D.enabled = false;
                    _swingingMovement2D.enabled = false;
                    break;
                case MovementType.Walking:
                    _rollingMovement2D.enabled = false;
                    _characterMovement2D.enabled = true;
                    _swingingMovement2D.enabled = false;
                    break;
                case MovementType.Swinging:
                    _rollingMovement2D.enabled = false;
                    _characterMovement2D.enabled = false;
                    _swingingMovement2D.enabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(movementType), movementType, null);
            }
        }

        public BaseLimb GetArm()
        {
            return limbs[ArmSlot];
        }

        public BaseLimb GetLeg()
        {
            return limbs[LegSlot];
        }

        public void SetArm(BaseLimb limb)
        {
            limbs[ArmSlot] = limb;
            if (limb == null) return;
            limb.transform.parent = transform;
            limb.transform.localPosition = new Vector3(0, limb.GetYOffset(), transform.position.z + 1);
            limb.SetPlayerManager(this);
        }

        public void SetLeg(BaseLimb limb)
        {
            limbs[LegSlot] = limb;
            if (limb == null) return;
            limb.transform.parent = transform;
            Transform limbTransform;
            (limbTransform = limb.transform).localPosition =
                new Vector3(0, -limb.GetYOffset(), transform.position.z + 1);
            limbTransform.localEulerAngles = new Vector3(0, 0, 180f);
            limb.SetPlayerManager(this);
        }

        public bool HasArm()
        {
            return GetArm() != null;
        }

        public bool HasLeg()
        {
            return GetLeg() != null;
        }

        void SwapLimbs()
        {
            CapsuleCollider2D capsuleCollider2D = (CapsuleCollider2D) _collider2D;
            float previousYOffset = capsuleCollider2D.offset.y;
            BaseLimb temp = GetArm();
            SetArm(GetLeg());
            SetLeg(temp);
            
            RefreshPlayer(previousYOffset);

            OnLimbSwap?.Invoke(limbs);
        }

        void RepositionLimbs()
        {
            BaseLimb leg = GetLeg();
            if (leg != null)
            {
                leg.transform.parent = transform;
                Transform legTransform;
                (legTransform = leg.transform).localPosition =
                    new Vector3(0, -leg.GetYOffset(), transform.position.z + 1);
                legTransform.localEulerAngles = new Vector3(0, 0, 180f);
            }

            BaseLimb arm = GetArm();
            if (arm != null)
            {
                arm.transform.parent = transform;
                arm.transform.localPosition = new Vector3(0, arm.GetYOffset(), transform.position.z + 1);
            }
        }

        Vector2[] GetColliderDimensions(BaseLimb arm, BaseLimb leg)
        {
            float armSize = arm != null ? arm.GetYOffset() : 0;
            float legSize = leg != null ? leg.GetYOffset() : 0;

            Vector2[] dimensions = new Vector2[2];

            dimensions[0] = new Vector2(1, 1 + armSize + legSize);

            if (armSize == legSize)
            {
                dimensions[1] = new Vector2(0, 0);
            }
            else
            {
                if (armSize > legSize)
                {
                    dimensions[1] = new Vector2(0, Mathf.Abs(legSize - armSize) / 2);
                }
                else
                {
                    dimensions[1] = new Vector2(0, -Mathf.Abs(armSize - legSize) / 2);
                }
            }

            return dimensions;
        }

        void AdjustCollider()
        {
            CapsuleCollider2D capsuleCollider2D = (CapsuleCollider2D) _collider2D;
            BaseLimb leg = GetLeg();
            BaseLimb arm = GetArm();

            Vector2[] dimensions = GetColliderDimensions(arm, leg);

            capsuleCollider2D.size = dimensions[0];
            capsuleCollider2D.offset = dimensions[1];
        }

        void AdjustCollider(float previousYOffset)
        {
            CapsuleCollider2D capsuleCollider2D = (CapsuleCollider2D) _collider2D;
            BaseLimb leg = GetLeg();
            BaseLimb arm = GetArm();

            Vector2[] dimensions = GetColliderDimensions(arm, leg);
            float currentYOffset = dimensions[1].y;
            float offsetDifference = previousYOffset - currentYOffset;

            Transform myTransform = transform;
            myTransform.position += myTransform.up * offsetDifference;

            capsuleCollider2D.size = dimensions[0];
            capsuleCollider2D.offset = dimensions[1];
        }

        void AdjustCamera()
        {
            //CameraFollow2D.GetInstance().SetOffset(((CapsuleCollider2D) _collider2D).offset);
        }

        private void Awake()
        {
            _characterMovement2D = GetComponent<CharacterMovement2D>();
            _rollingMovement2D = GetComponent<RollingMovement2D>();
            _swingingMovement2D = GetComponent<SwingingMovement2D>();
            _collider2D = GetComponent<Collider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();

            if (this.testLeg != null)
            {
                GameObject leg = Instantiate(this.testLeg);
                SetLeg(leg.GetComponent<BaseLimb>());
            }

            SetupLimbs();
        }

        void RefreshPlayer()
        {
            //RepositionLimbs();
            UpdateMovementType();
            AdjustCollider();
            AdjustCamera();
        }
        
        void RefreshPlayer(float previousYOffset)
        {
            UpdateMovementType();
            //RepositionLimbs();
            AdjustCollider(previousYOffset);
            AdjustCamera();
        }
        
        void AttachLimb(BaseLimb limb)
        {
            if (limb.CanBeLeg() && limb.CanBeArm())
            {
                if (!HasLeg())
                {
                    SetLeg(limb);
                }
                else if (!HasArm())
                {
                    SetArm(limb);
                }
            }
            else if (limb.CanBeLeg() && !HasLeg())
            {
                SetLeg(limb);
            }
            else if (limb.CanBeArm() && !HasArm())
            {
                SetArm(limb);
            }

            RefreshPlayer();
        }

        void SetupLimbs()
        {
            RepositionLimbs();
            RefreshPlayer();
        }

        void UpdateMovementType()
        {
            MovementType movementType = GetMovementType();
            if (movementType != MovementType.Swinging)
            {
                BaseLimb leg = GetLeg();
                SetMovementType( leg != null && leg.CanBeLeg() ? MovementType.Walking : MovementType.Rolling);
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SwapLimbs();

            if (Input.GetMouseButton(0))
                GetArm()?.ArmPrimary();
        }

        public Rigidbody2D GetRigidbody2D()
        {
            return _rigidbody2D;
        }
    }
}
