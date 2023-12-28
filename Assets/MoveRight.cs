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

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody.velocity = Vector2.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        // Find distance to next car

        RaycastHit2D hit = Physics2D.Raycast(myRigidBody.position, myRigidBody.velocity, 100f, layerMask);
        
        if (hit)
        {

            // Determine if current velocity, acceleration, and distance are okay
            float myVelocity = myRigidBody.velocity.magnitude;
            float hitVelocity = hit.rigidbody.velocity.magnitude;

            float distance = hit.distance - myCollider.bounds.size.x / 2f;

            Debug.Log(hit.distance);


        }



        // Update acceleration if not
        if (Math.Abs(transform.position.x) > 70)
        {
            Destroy(gameObject);
        }
    }
}
