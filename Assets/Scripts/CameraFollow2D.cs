using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class CameraFollow2D : MonoBehaviour
{
    private static CameraFollow2D _instance;

    [SerializeField]
    private GameObject trackedObject;
    [SerializeField]
    private float layersOffset;
    [SerializeField]
    private Vector2 offset;
    [SerializeField]
    private float minX = -100f;
    [SerializeField]
    private float maxX = 100f;

    private SpriteRenderer _targetSpriteRenderer;

    private void Awake()
    {
        _instance = this;
        if (trackedObject == null) return;
        _targetSpriteRenderer = trackedObject.GetComponent<SpriteRenderer>();
    }

    public static CameraFollow2D GetInstance()
    {
        return _instance;
    }

    void Update()
    {
        if (trackedObject == null) return;
        Vector3 position = _targetSpriteRenderer.bounds.center;
        transform.position = new Vector3(Mathf.Clamp(position.x + offset.x, minX, maxX), position.y + offset.y, -layersOffset);
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

    public void SetOffset(Vector2 newOffset)
    {
        this.offset = newOffset;
    }    

    public Vector2 GetOffset()
    {
        return this.offset;
    }
}
