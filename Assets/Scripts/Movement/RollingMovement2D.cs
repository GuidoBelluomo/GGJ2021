using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingMovement2D : Movement2D
{
    [SerializeField]
    float accelerationFactor = 1.5f;
    [SerializeField]
    float decelerationFactor = 2.5f;
    [SerializeField]
    float sleepThreshold = 0.01f;
    [SerializeField]
    float speedGain = 10;
    [SerializeField]
    float maxSpeed = 10;

    Rigidbody2D rigidbody2d;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(rigidbody2d.angularVelocity) < maxSpeed)
        {
            float h = -Input.GetAxisRaw("Horizontal");
            if (Mathf.Sign(h) + Mathf.Sign(rigidbody2d.angularVelocity) != 0)
                rigidbody2d.angularVelocity += h * speedGain * accelerationFactor * Time.deltaTime;
            else
                rigidbody2d.angularVelocity += h * speedGain * decelerationFactor * Time.deltaTime;
        }
    }
}
