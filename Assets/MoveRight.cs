using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRight : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
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
        RaycastHit2D hit = Physics2D.Raycast(myRigidBody.position, myRigidBody.velocity, 100f, layerMask);
        
        if (hit)
        {
            Debug.Log(hit.rigidbody.velocity);
        }

        // Find distance to next car

        // Determine if current velocity, acceleration, and distance are okay
        // Update acceleration if not
        if (Math.Abs(transform.position.x) > 70)
        {
            Destroy(gameObject);
        }
    }
}
