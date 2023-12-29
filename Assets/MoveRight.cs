using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MoveRight : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public Collider2D myCollider;
    public LayerMask layerMask;

    public float speedLimit = 10.0f;
    public float desiredVelocity = 10.0f;
    public float minimumSpacing = 1.5f;
    public float desiredTimeGap = 2;
    public float maxAcceleration = 3;
    public float comfyBrakingDecelleration = 3f;
    public float exponent = 4;


    // Start is called before the first frame update
    void Start()
    {
        myRigidBody.velocity = Vector2.right * speedLimit;
    }

    // Update is called once per frame
    void Update()
    {
        // Find distance to next car

        // Notes:
        // 1 distance = 1 meter
        // 1 meter/second = 3.6 km/hour
        // 1 mph = 1.609 km/hour
        // 1 meter/second = 2.237 mph

        // Here I will use the Intelligent Driver Model (IDM) as outlined
        // on a Wikipedia page https://en.wikipedia.org/wiki/Intelligent_driver_model.

        float accelerationFree = maxAcceleration * (1 - Mathf.Pow((myRigidBody.velocity.magnitude / desiredVelocity), exponent));


        // Adjusts acceleration if any cars are ahead
        RaycastHit2D hit = Physics2D.Raycast(myRigidBody.position, myRigidBody.velocity, 100f, layerMask);

        if (hit)
        {
            float netDistance = hit.distance - (myCollider.bounds.size.x / 2f);

            float approachVelocity = myRigidBody.velocity.magnitude - hit.rigidbody.velocity.magnitude;

            float function = minimumSpacing + (myRigidBody.velocity.magnitude * desiredTimeGap) + ((myRigidBody.velocity.magnitude * approachVelocity) / (2 * netDistance * Mathf.Sqrt(maxAcceleration * comfyBrakingDecelleration)));

            float adjustment = maxAcceleration * Mathf.Pow(function / netDistance, 2);

            accelerationFree -= adjustment;
        }

        // Applies acceleration
        myRigidBody.velocity = new Vector2(myRigidBody.velocity.magnitude + (accelerationFree * Time.deltaTime), 0);


        if (Math.Abs(transform.position.x) > 70)
        {
            Destroy(gameObject);
        }
    }
}
