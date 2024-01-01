using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Directions are relative to 2 Way Stop object oriented with 0 rotation

public class TwoWayStopScript : MonoBehaviour
{
    // These names may be misleading if parent object is rotated
    public GameObject posXRanger;
    public GameObject negXRanger;

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

        Rigidbody2D closestPosX = posXHit.rigidbody;
        Rigidbody2D closestNegX = negXHit.rigidbody;

        try
        {
            Debug.Log(closestPosX.velocity);

        }
        catch (System.NullReferenceException e)
        {
            return;
        }


    }
}
