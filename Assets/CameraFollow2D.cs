using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class CameraFollow2D : MonoBehaviour
{
    static CameraFollow2D instance;

    [SerializeField]
    private GameObject trackedObject;
    [SerializeField]
    private float layersOffset;
    [SerializeField]
    private Vector2 offset;

    private void Awake()
    {
        instance = this;
    }

    public static CameraFollow2D GetInstance()
    {
        return instance;
    }

    void LateUpdate()
    {
        if (trackedObject != null)
            transform.position = new Vector3(trackedObject.transform.position.x + offset.x, trackedObject.transform.position.y + offset.y, -layersOffset);
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

    public void SetOffset(Vector2 offset)
    {
        this.offset = offset;
    }    

    public Vector2 GetOffset()
    {
        return this.offset;
    }
}
