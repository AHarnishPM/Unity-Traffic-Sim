using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MoveRight : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public Collider2D myCollider;
    public float speed = 10.0f;

    public LayerMask layerMask;
    public float acceleration = 0;


    // Start is called before the first frame update
    void Start()
    {
        myRigidBody.velocity = Vector2.right * speed;
        Debug.Log(myRigidBody.rotation);
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

        RaycastHit2D hit = Physics2D.Raycast(myRigidBody.position, myRigidBody.velocity, 100f, layerMask);

        if (hit)
        {

            // Determine if current velocity, acceleration, and distance are okay
            float myVelocity = myRigidBody.velocity.magnitude;
            float hitVelocity = hit.rigidbody.velocity.magnitude;

            float distance = hit.distance - myCollider.bounds.size.x / 2f;

            float safeFollowingDistance = 1f + (hitVelocity * 2);

            // If my current settings and the other object's velocity will result in an unsafe following distance in the
            // next three seconds, adjust

            float myDisplacementAfter3 = (myVelocity * 3) + (0.5f * acceleration * 9f);
            float otherDisplacementAfter3 = hitVelocity * 3;
            float distanceAfter3Seconds = distance - myDisplacementAfter3 + otherDisplacementAfter3;

            // if speed and distance are below a threshold, come to full stop.

            Debug.Log("Velocity = " + myVelocity);


            if (distance < 1 && myRigidBody.velocity.magnitude < 3)
            {
                myRigidBody.velocity = Vector2.zero;
            }


            else if (distanceAfter3Seconds < safeFollowingDistance) 
            {
                Debug.Log(myRigidBody + " Slowing Down");

                //Brake so that my final velocity = the velocity of car in front of me
                //and my final position is the correct following distance away
                // vFinal^2 = vInitial^2 + 2ad
                // acceleration = (vFinal^2 - vInitial^2) / 2d
                acceleration = (Mathf.Pow(hitVelocity, 2) - Mathf.Pow(myVelocity, 2)) / (distance * 2f);

                // If object is closer than following distance and current trajectory will hit it, slow down
                // If furhter than following distance, speed up to speed limit.

                myRigidBody.velocity = new Vector2(myRigidBody.velocity.x + (acceleration), myRigidBody.velocity.y);
            }
        }



        // Update acceleration if not
        if (Math.Abs(transform.position.x) > 70)
        {
            Destroy(gameObject);
        }
    }
}
