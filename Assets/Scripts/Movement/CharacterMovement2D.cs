using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement2D : Movement2D
{
    [SerializeField]
    float accelerationFactor = 1;
    [SerializeField]
    float groundedTolerance = 0.05f;
    [SerializeField]
    float decelerationFactor = 1;
    [SerializeField]
    float maxSpeed = 10;
    [SerializeField]
    float acceleration = 10;
    [SerializeField]
    float airControlEfficiency = 1f;
    [SerializeField]
    float jumpForce = 10;

    [SerializeField]
    Vector2 movement;
    Vector2 projectedMovement;
    Vector2 gravity;
    Vector2 groundNormal = Vector3.up;


    new Collider2D collider2D;

    [SerializeField]
    bool grounded;
    
    Rigidbody2D rigidbody2d;
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
    }

    void AirMovement(float h)
    {
        gravity += Vector2.down * 9.81f * Time.deltaTime;

        Vector2 accelerationVector = new Vector2(h, 0) * Time.deltaTime * acceleration * accelerationFactor * airControlEfficiency;
        movement += accelerationVector;
        if (movement.magnitude > maxSpeed)
            movement = movement.normalized * maxSpeed;
    }

    void Jump()
    {
        groundNormal = Vector3.up;
        gravity = transform.up * jumpForce;
        grounded = false;
    }

    void Accelerate(float h)
    {
        Vector2 accelerationVector = new Vector2(h, 0) * Time.deltaTime * acceleration * accelerationFactor;
        movement += accelerationVector;
        if (movement.magnitude > maxSpeed)
            movement = movement.normalized * maxSpeed;
    }

    void Decelerate(float h)
    {
        Vector2 accelerationVector = new Vector2(h, 0) * Time.deltaTime * acceleration * decelerationFactor;
        movement += accelerationVector;
        if (movement.magnitude > maxSpeed)
            movement = movement.normalized * maxSpeed;
    }

    void DecelerateToStop()
    {
        float previousXSign = Mathf.Sign(movement.x);
        float previousYSign = Mathf.Sign(movement.y);

        movement -= movement.normalized * acceleration * decelerationFactor * Time.deltaTime;

        float currentXSign = Mathf.Sign(movement.x);
        float currentYSign = Mathf.Sign(movement.y);

        if (currentXSign != previousXSign)
            movement.x = 0;

        if (currentYSign != previousYSign)
            movement.x = 0;
    }

    void Move(float h)
    {
        if (!grounded)
        {
            AirMovement(h);
        }
        else
        {
            if (h != 0)
            {
                if ((Mathf.Sign(h) + Mathf.Sign(movement.x)) == 0)
                {
                    Decelerate(h);
                }
                else
                {
                    Accelerate(h);
                }
            }
            else
            {
                DecelerateToStop();
            }
        }

        projectedMovement = Quaternion.FromToRotation(transform.up, groundNormal) * movement;
        rigidbody2d.velocity = projectedMovement + gravity;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = GroundCast();

        if (Input.GetButtonDown("Jump"))
            Jump();

        float h = Input.GetAxisRaw("Horizontal");
        Move(h);
    }

    bool GroundCast()
    {
        if (gravity.y > 0)
            return false;

        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = 1 << 6;
        RaycastHit2D[] results = new RaycastHit2D[1];
        int hits = ((CircleCollider2D)collider2D).Cast(Vector2.down, filter, results, groundedTolerance);
        if (hits > 0)
        {
            if (!grounded)
            {
                transform.position = results[0].centroid;
                gravity = Vector2.zero;
                groundNormal = results[0].normal;
            }
            return true;
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GroundCast())
        {
            grounded = true;
        }

        if (ShouldKillSpeed())
        {
            movement = Vector2.zero;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (ShouldKillSpeed())
        {
            movement = Vector2.zero;
        }
    }

    bool ShouldKillSpeed()
    {
        Vector2 source = (Vector2)transform.position;
        Vector2 dir = (collider2D.bounds.extents.x + 0.05f) * projectedMovement.normalized;

        RaycastHit2D hit = Physics2D.Linecast(source, source + dir, 1 << 6);

        source = (Vector2)transform.position + (collider2D.bounds.extents.y * groundNormal * 0.2f);
        RaycastHit2D hit2 = Physics2D.Linecast(source, source + dir, 1 << 6);

        source = (Vector2)transform.position - (collider2D.bounds.extents.y * groundNormal * 0.2f);
        RaycastHit2D hit3 = Physics2D.Linecast(source, source + dir, 1 << 6);

        return hit.collider != null || hit2.collider != null || hit3.collider != null;
    }
}
