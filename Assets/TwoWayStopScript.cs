using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Directions are relative to 2 Way Stop object oriented with 0 rotation
// Following suggestions from https://www.dmv.pa.gov/Driver-Services/Driver-Licensing/Driver-Manual/Chapter-3/Everyday-Driving/Pages/Negotiating-Intersections.aspx

public class TwoWayStopScript : MonoBehaviour
{
    // These names may be misleading if parent object is rotated
    public GameObject posXRanger;
    public GameObject negXRanger;

    // These are used for the stopping lanes
    public GameObject posYRanger;
    public GameObject negYRanger;

    public float stoppedSpeed;
    public float stoppedDistance;
    public float timeToPullOut;

    public LayerMask noBarriersMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Collect first car data for all incoming lanes
        RaycastHit2D posXHit = posXRanger.GetComponent<SingleSensorScript>().hit;
        RaycastHit2D negXHit = negXRanger.GetComponent<SingleSensorScript>().hit;
        RaycastHit2D posYHit = posYRanger.GetComponent<SingleSensorScript>().hit;
        RaycastHit2D negYHit = negYRanger.GetComponent<SingleSensorScript>().hit;

        Rigidbody2D bodyPosX = posXHit.rigidbody; 
        Rigidbody2D bodyNegX = negXHit.rigidbody;
        Rigidbody2D bodyPosY = posYHit.rigidbody;
        Rigidbody2D bodyNegY = negYHit.rigidbody;

        //bool topWaiting = !((posYHit == false) || !((bodyPosY.velocity.magnitude < stoppedSpeed) && (posYHit.distance < stoppedDistance)));
        bool topWaiting = posYHit.collider != null && ((bodyPosY.velocity.magnitude < stoppedSpeed) && (Mathf.Abs(posYHit.distance) < stoppedDistance));
        bool bottomWaiting = negYHit.collider != null && ((bodyNegY.velocity.magnitude < stoppedSpeed) && (Mathf.Abs(negYHit.distance) < stoppedDistance));


        //bool bottomWaiting = bodyNegY.velocity.magnitude < stoppedSpeed && negYHit.distance < stoppedDistance;

        if (topWaiting || bottomWaiting)
        {
            // Recommended 6 second gap for a 30 mph street
            bool posXClear = posXHit.collider == null || posXHit.distance > bodyPosX.velocity.magnitude * timeToPullOut;
            bool negXClear = negXHit.collider == null || negXHit.distance > bodyNegX.velocity.magnitude * timeToPullOut;


            if (posXClear && negXClear)
            {
                if (topWaiting)
                {
                    // Allow the car to ignore the barrier
                    var topScript = posYHit.collider.gameObject.GetComponent<MoveRight>();
                    topScript.layerMask = noBarriersMask;
                }
                if (bottomWaiting)
                {
                    // Allow the car to ignore the barrier
                    var bottomScript = negYHit.collider.gameObject.GetComponent<MoveRight>();
                    bottomScript.layerMask = noBarriersMask;
                }
                

            }
        }
        





    }
}
