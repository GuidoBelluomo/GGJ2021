using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        var transform1 = transform;
        var position = transform1.position;
        position = new Vector3(Camera.main.transform.position.x / 2, Camera.main.transform.position.y / 2, position.z);
        transform1.position = position;
    }
}
