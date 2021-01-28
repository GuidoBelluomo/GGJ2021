using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseLimb : MonoBehaviour
{
    private PlayerManager playerManager;
    [SerializeField]
    private bool canBeLeg = false;
    [SerializeField]
    private bool canBeArm = true;
    [SerializeField]
    protected float yOffset = 0.8f;

    public PlayerManager GetPlayerManager()
    {
        return playerManager;
    }

    public void SetPlayerManager(PlayerManager playerManager)
    {
        this.playerManager = playerManager;
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
        ARM,
        LEG
    }

    public virtual void ArmPrimary()
    {

    }

    public virtual void LegPrimary()
    {

    }

    public void Primary(Slot slot)
    {
        switch (slot)
        {
            case Slot.ARM:
                ArmPrimary();
                break;
            case Slot.LEG:
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
            case Slot.ARM:
                ArmSecondary();
                break;
            case Slot.LEG:
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
