using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepUpright : MonoBehaviour
{
    [SerializeField]
    private bool active = true;

    public void SetEnabled(bool enabled)
    {
        this.active = enabled;
    }

    public bool GetEnabled()
    {
        return active;
    }

    void Update()
    {
        if (!active) return;
        transform.eulerAngles = Vector3.zero;
    }
}
