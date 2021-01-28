using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class CameraFollow2D : MonoBehaviour
{
    [SerializeField]
    private GameObject trackedObject;
    [SerializeField]
    private float layersOffset;

    void LateUpdate()
    {
        if (trackedObject != null)
            transform.position = new Vector3(trackedObject.transform.position.x, trackedObject.transform.position.y, -layersOffset);
    }

    public void SetTrackedObject(GameObject newObj)
    {
        trackedObject = newObj;
    }

    public GameObject GetTrackedObject()
    {
        return trackedObject;
    }

    public void SetDistance(float newDist)
    {
        layersOffset = newDist;
    }

    public float GetDistance()
    {
        return layersOffset;
    }
}
