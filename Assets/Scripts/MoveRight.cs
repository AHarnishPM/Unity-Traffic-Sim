using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRight : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public float speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody.velocity = Vector2.right * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Math.Abs(transform.position.x) > 70)
        {
            Destroy(gameObject);
        }
    }
}
