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
    public LayerMask maskWithBarriers;
    public GameObject onElement;
    public GameObject leftSignal;
    public GameObject rightSignal;

    
    public float minimumSpacing = 1.5f;
    public float desiredTimeGap = 2;
    public float maxAcceleration = 3.8f;
    public float comfyBrakingDecelleration = 2.5f;
    public float exponent = 4;
    public float timer = 0;

    public int turnSignal; // -1 left, 0 straight, 1 right

    public float speedLimit;
    public float desiredVelocity;


    // Start is called before the first frame update
    void Start()
    {
        desiredVelocity = speedLimit;
        myRigidBody.velocity = transform.rotation * Vector2.up * desiredVelocity;
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
        RaycastHit2D hit = Physics2D.Raycast(myRigidBody.position, myRigidBody.velocity, 200f, layerMask);

        if (hit.collider != null)
        {
            float netDistance = hit.distance - (myCollider.bounds.size.x / 2f);

            float approachVelocity = myRigidBody.velocity.magnitude - hit.rigidbody.velocity.magnitude;

            float function = minimumSpacing + (myRigidBody.velocity.magnitude * desiredTimeGap) + ((myRigidBody.velocity.magnitude * approachVelocity) / (2 * netDistance * Mathf.Sqrt(maxAcceleration * comfyBrakingDecelleration)));

            float adjustment = maxAcceleration * Mathf.Pow(function / netDistance, 2);

            accelerationFree -= adjustment;
        }

        // Applies acceleration
        myRigidBody.velocity *= (myRigidBody.velocity.magnitude + (accelerationFree * Time.deltaTime)) / myRigidBody.velocity.magnitude;


        if (Math.Abs(transform.position.x) > 185 || Math.Abs(transform.position.y) > 105)
        {
            Destroy(gameObject);
        }

        // turn signal
        if (timer < 0.4f)
        {
            timer += Time.deltaTime;
        }
        else if (turnSignal != 0)
        {
            timer = 0;
            GameObject blinker = (turnSignal > 0) ? rightSignal : leftSignal;
            blinker.SetActive(blinker.activeSelf ? false : true);
        }



    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onElement = collision.gameObject;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        layerMask = maskWithBarriers;
    }
}
