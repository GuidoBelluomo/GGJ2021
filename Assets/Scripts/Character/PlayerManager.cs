using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public const int ARM_SLOT = (int)BaseLimb.Slot.ARM;
    public const int LEG_SLOT = (int)BaseLimb.Slot.LEG;

    private BaseLimb[] limbs = new BaseLimb[2] { null, null };

    public delegate void LimbSwap(BaseLimb[] limbs);
    public event LimbSwap OnLimbSwap;

    [SerializeField]
    GameObject arm;
    [SerializeField]
    GameObject leg;

    new Collider2D collider2D;

    public BaseLimb GetArm()
    {
        return limbs[ARM_SLOT];
    }

    public BaseLimb GetLeg()
    {
        return limbs[LEG_SLOT];
    }

    public void SetArm(BaseLimb limb)
    {
        limbs[ARM_SLOT] = limb;
        limb.transform.parent = transform;
        limb.transform.localPosition = new Vector3(0, limb.GetYOffset(), transform.position.z + 1);
    }

    public void SetLeg(BaseLimb limb)
    {
        limbs[LEG_SLOT] = limb;
        limb.transform.parent = transform;
        limb.transform.localPosition = new Vector3(0, -limb.GetYOffset(), transform.position.z + 1);
        limb.transform.localEulerAngles = new Vector3(0, 0, 180f);
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
        CapsuleCollider2D capsuleCollider2D = (CapsuleCollider2D)collider2D;
        float previousYOffset = capsuleCollider2D.offset.y;
        BaseLimb temp = GetArm();
        SetArm(GetLeg());
        SetLeg(temp);

        RepositionLimbs(limbs);
        AdjustCollider(limbs, previousYOffset);
        AdjustCamera();

        if (OnLimbSwap != null)
            OnLimbSwap(limbs);
    }

    void RepositionLimbs(BaseLimb[] limbs)
    {   
        BaseLimb leg = GetLeg();
        if (leg != null)
        {
            leg.transform.parent = transform;
            leg.transform.localPosition = new Vector3(0, -leg.GetYOffset(), transform.position.z + 1);
            leg.transform.localEulerAngles = new Vector3(0, 0, 180f);
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
        float armSize = arm?.GetYOffset() ?? 0;
        float legSize = leg?.GetYOffset() ?? 0;

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

    void AdjustCollider(BaseLimb[] limbs)
    {
        CapsuleCollider2D capsuleCollider2D = (CapsuleCollider2D)collider2D;
        BaseLimb leg = GetLeg();
        BaseLimb arm = GetArm();

        Vector2[] dimensions = GetColliderDimensions(arm, leg);

        capsuleCollider2D.size = dimensions[0];
        capsuleCollider2D.offset = dimensions[1];
    }

    void AdjustCollider(BaseLimb[] limbs, float previousYOffset)
    {
        CapsuleCollider2D capsuleCollider2D = (CapsuleCollider2D)collider2D;
        BaseLimb leg = GetLeg();
        BaseLimb arm = GetArm();

        Vector2[] dimensions = GetColliderDimensions(arm, leg);
        float currentYOffset = dimensions[1].y;
        float offsetDifference = previousYOffset - currentYOffset;

        transform.position = new Vector3(transform.position.x, transform.position.y + offsetDifference, transform.position.z);

        capsuleCollider2D.size = dimensions[0];
        capsuleCollider2D.offset = dimensions[1];
    }

    void AdjustCamera()
    {
        CameraFollow2D.GetInstance().SetOffset(((CapsuleCollider2D)collider2D).offset);
    }

    private void Awake()
    {
        collider2D = GetComponent<Collider2D>();


        if (this.leg != null)
        {
            GameObject leg = Instantiate(this.leg);
            SetLeg(leg.GetComponent<BaseLimb>());
        }

        SetupLimbs();
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
        AdjustCollider(limbs);
        AdjustCamera();
    }

    void SetupLimbs()
    {
        RepositionLimbs(limbs);
        AdjustCollider(limbs);
        AdjustCamera();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            SwapLimbs();

        if (Input.GetMouseButton(0))
            GetArm()?.ArmPrimary();
    }
}
