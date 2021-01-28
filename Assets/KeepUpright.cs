using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepUpright : MonoBehaviour
{
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
        transform.eulerAngles = Vector3.zero;
    }
}
