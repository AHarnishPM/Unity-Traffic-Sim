using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveRight : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public Collider2D myCollider;
    public LayerMask layerMask;
    public LayerMask maskLightBarriers;
    public LayerMask maskNoBarriers;
    public GameObject leftSignal;
    public GameObject rightSignal;

    
    public float minimumSpacing = 1.5f;
    public float desiredTimeGap = 2;
    public float maxAcceleration = 3.8f;
    public float comfyBrakingDecelleration = 2.5f;
    public float exponent = 4;
    public float timer = 0;
    public bool isTurning = false;

    public int turnSignal = 1; // -1 left, 0 straight, 1 right
    private bool lookForTPs = true;


    public float speedLimit;
    public float desiredVelocity;
    public bool hasYellowClearance = false;
    public bool hasRedClearance = false;
    public bool hasLeftClearance = false;

    //public float accelerationFree;
    //public float accelerationAdj;


    // Start is called before the first frame update
    void Start()
    {
        desiredVelocity = speedLimit;
        myRigidBody.velocity = transform.rotation * Vector2.up * desiredVelocity;
        layerMask = maskLightBarriers;
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

        // TODO: Better versions of this model exist, should probably upgrade.
        // NOTE: used improvement where velocity is always non negative

        //float myVelocity = myRigidBody.velocity.magnitude;

        //if (Vector3.Dot(new Vector3(myRigidBody.velocity.x, myRigidBody.velocity.y), transform.up) < 0)
        //{
        //    myVelocity = 0;
        //}

        float accelerationFree = maxAcceleration * (1 - Mathf.Pow((myRigidBody.velocity.magnitude / desiredVelocity), exponent));

        // Adjusts acceleration if any cars are ahead
        RaycastHit2D hit = Physics2D.Raycast(myRigidBody.position, myRigidBody.velocity, 200f, layerMask);

        if (hit.collider != null && !isTurning)
        {
            float netDistance = hit.distance - (myCollider.bounds.size.x / 2f);

            float approachVelocity = myRigidBody.velocity.magnitude - hit.rigidbody.velocity.magnitude;

            float function = minimumSpacing + (myRigidBody.velocity.magnitude * desiredTimeGap) + ((myRigidBody.velocity.magnitude * approachVelocity) / (2 * netDistance * Mathf.Sqrt(maxAcceleration * comfyBrakingDecelleration)));

            float adjustment = maxAcceleration * Mathf.Pow(function / netDistance, 2);

            accelerationFree -= adjustment;
        }

        // Applies acceleration
        myRigidBody.velocity = (myRigidBody.velocity.magnitude + (accelerationFree * Time.deltaTime)) * transform.up;

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
    // Car ignores barriers, used to let through stop signs and yellow lights.
    public void ignoreBarriers()
    {
        layerMask = maskNoBarriers;
    }

    public void recognizeBarriers()
    {
        layerMask = maskLightBarriers;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTurning)
        {
            this.gameObject.layer = 8;
        }
        
    }

    // After a car passes through the intersection, stop ignoring barriers.
    private void OnTriggerExit2D(Collider2D other)
    {
        layerMask = maskLightBarriers;
        lookForTPs = true;
        hasRedClearance = false;
        hasYellowClearance = false;
        hasLeftClearance = false;

        if (!isTurning)
        {
            this.gameObject.layer = 6;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        float distanceBetween = (collision.transform.position - this.transform.position).magnitude;

        if (lookForTPs && distanceBetween < 0.5f)
        {
            transform.position = collision.transform.position;
            tryTurn(turnSignal);
        }
    }

    private void tryTurn(int turnSignal)
    {
        if (turnSignal == -1) {
            transform.Rotate(0f, 0f, 45f);
            isTurning = (isTurning) ? false : true;
        }

        else if (turnSignal == 1)
        {
            transform.Rotate(0f, 0f, -45f);
            isTurning = (isTurning) ? false : true;
        }
        myRigidBody.velocity = transform.rotation * Vector2.up * myRigidBody.velocity.magnitude;


        lookForTPs = false;
    }
}
