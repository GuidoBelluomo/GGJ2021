using System;
using Limbs;
using Movement;
using Objects;
using Objects.Interactables;
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
        private KeepUpright _keepUpright;
        
        public Vector2 GetBottomPosition()
        {
            Bounds colliderBounds = _collider2D.bounds;
            return (Vector2)colliderBounds.center - new Vector2(0, colliderBounds.extents.y);
        }
        
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
            Transform limbTransform;
            (limbTransform = limb.transform).parent = transform;
            limb.GetRigidbody2D().bodyType = RigidbodyType2D.Kinematic;
            limb.GetRigidbody2D().velocity = Vector2.zero;
            limb.GetRigidbody2D().angularVelocity = 0;
            limbTransform.localPosition = new Vector3(0, limb.GetYOffset(), transform.position.z + 1);
            limbTransform.localEulerAngles = new Vector3(0, 0, 0);
            limb.SetPlayerManager(this);
        }

        public void SetLeg(BaseLimb limb)
        {
            limbs[LegSlot] = limb;
            if (limb == null) return;
            limb.GetRigidbody2D().bodyType = RigidbodyType2D.Kinematic;
            limb.GetRigidbody2D().velocity = Vector2.zero;
            limb.GetRigidbody2D().angularVelocity = 0;
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
            if (!_characterMovement2D.enabled) return;
            
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
            _characterMovement2D.SetPlayerManager(this);
            _rollingMovement2D.SetPlayerManager(this);
            _swingingMovement2D.SetPlayerManager(this);
            _collider2D = GetComponent<Collider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _keepUpright = GetComponentInChildren<KeepUpright>();

            if (this.testLeg != null)
            {
                GameObject leg = Instantiate(this.testLeg);
                SetLeg(leg.GetComponent<BaseLimb>());
            }
            
            if (this.testArm != null)
            {
                GameObject arm = Instantiate(this.testArm);
                SetArm(arm.GetComponent<BaseLimb>());
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
        
        bool AttachLimb(BaseLimb limb)
        {
            if (limb == null) return false;
            
            if (limb.CanBeLeg() && limb.CanBeArm())
            {
                if (!HasLeg())
                {
                    SetLeg(limb);
                    RefreshPlayer();
                    return true;
                }
                else if (!HasArm())
                {
                    SetArm(limb);
                    RefreshPlayer();
                    return true;
                }
            }
            else if (limb.CanBeLeg() && !HasLeg())
            {
                SetLeg(limb);
                RefreshPlayer();
                return true;
            }
            else if (limb.CanBeArm() && !HasArm())
            {
                SetArm(limb);
                RefreshPlayer();
                return true;
            }

            return false;
        }

        void SetupLimbs()
        {
            RepositionLimbs();
            RefreshPlayer();
        }

        public void UpdateMovementType(bool forced = false)
        {
            MovementType movementType = GetMovementType();
            if (forced || movementType != MovementType.Swinging)
            {
                BaseLimb leg = GetLeg();
                BaseLimb arm = GetArm();
                SetMovementType( leg != null || arm != null ? MovementType.Walking : MovementType.Rolling);
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                SwapLimbs();

            if (Input.GetButtonDown("Swing"))
                GetArm()?.ArmSecondary();
            
            if (Input.GetButtonDown("Toss"))
                GetArm()?.Toss();

            if (Input.GetButtonDown("Interact"))
            {
                if (AttachLimb(BaseLimb.GetClosestLimb(GetBottomPosition(), 1f))) return;
                
                InteractableObject interactable = InteractableObject.GetClosestInteractable(GetBottomPosition(), 1f);
                if (interactable == null) return;
                interactable.OnInteracted.Invoke();
            }
        }

        public Rigidbody2D GetRigidbody2D()
        {
            return _rigidbody2D;
        }

        public void UnsetLimb(BaseLimb baseLimb)
        {
            if (baseLimb == null) return;

            baseLimb.transform.parent = null;
            Rigidbody2D limbRigidbody = baseLimb.GetRigidbody2D();
            limbRigidbody.bodyType = RigidbodyType2D.Dynamic;

            if (baseLimb == GetArm())
                SetArm(null);
            else if (baseLimb == GetLeg())
                SetLeg(null);
            
            RefreshPlayer();
        }

        public CharacterMovement2D GetCharacterMovement2D()
        {
            return _characterMovement2D;
        }
        
        public RollingMovement2D GetRollingMovement2D()
        {
            return _rollingMovement2D;
        }
        
        public KeepUpright GetKeepUpright()
        {
            return _keepUpright;
        }
    }
}
