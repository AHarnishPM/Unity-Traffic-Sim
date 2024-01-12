using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class StandardLightScript : MonoBehaviour
{
    public GameObject[] barriers = new GameObject[4];

    public GameObject[] multiRangers = new GameObject[4];

    public LightBarrierScript[] barrierScripts = new LightBarrierScript[4];

    public MultipleSensorScript[] rangerScripts = new MultipleSensorScript[4];

    public int[] currentOrientation = { 0, 0, 0, 0 };

    public int[][] orientations =
    {
        new int[] {0, 0, 0, 0 }, // All blocked, default orientation
        new int[] { 0, 2, 0, 2 }, // Horizontal free, vertical blocked
        new int[] { 2, 0, 2, 0 }, // Vertical free, horizontal blocked
        new int[] { 2, 0, 0, 0 }, // Top open
        new int[] { 0, 2, 0, 0 }, // Left open
        new int[] { 0, 0, 2, 0 }, // Bottom open
        new int[] { 0, 0, 0, 2 }  // Right open
    };

    private bool isRunning;

    public float leftGreenGap = 5;

    public float rightRedGap = 5;

    private float lightExtents;

    private float barrierHeight;

    // Start is called before the first frame update
    void Start()
    {
        for (int count = 0; count < barriers.Length; count++)
        {
            barrierScripts[count] = barriers[count].GetComponent<LightBarrierScript>();
            rangerScripts[count] = multiRangers[count].GetComponent<MultipleSensorScript>();
        }

        isRunning = true;

        lightExtents = this.GetComponent<BoxCollider2D>().bounds.extents.x;
        barrierHeight = barriers[0].GetComponent<BoxCollider2D>().bounds.extents.y * 2;

        startCycle();

    }

    // Update is called once per frame
    void Update()
    {
        // Tell all cars whether to ignore or respect barriers.

        // For each ranger
        for (int i = 0; i < multiRangers.Length; i++)
        {
            // For each car hit in ranger FOV
            foreach (RaycastHit2D hit in rangerScripts[i].hitList)
            {
                GameObject car = hit.collider.gameObject;

                MoveRight carScript = car.GetComponent<MoveRight>();

                // If light is green or would make it through on yellow
                if (barrierScripts[i].lightStatus == 2 || (barrierScripts[i].lightStatus == 1 && carScript.hasYellowClearance))
                {
                    // If the car is turning left
                    if (carScript.turnSignal == -1)
                    {
                        // If there are cars in the opposite lane
                        if (rangerScripts[(i+2)%4].numHits > 0)
                        {
                            List<RaycastHit2D> oppHits = rangerScripts[(i + 2) % 4].hitList;

                            bool canTurn = true;

                            foreach(RaycastHit2D oppHit in oppHits)
                            {
                                // If any opposing cars are not turning left and will reach the intersection less than leftGreenGap seconds after car, can't turn.
                                GameObject oppCar = oppHit.collider.gameObject;
                                MoveRight oppScript = oppCar.GetComponent<MoveRight>();

                                if (oppScript.turnSignal != -1)
                                {
                                    float oppVelo = oppCar.GetComponent<Rigidbody2D>().velocity.magnitude;
                                    float myVelo = car.GetComponent<Rigidbody2D>().velocity.magnitude;

                                    float oppTime = oppHit.distance / oppCar.GetComponent<Rigidbody2D>().velocity.magnitude;
                                    float myTime = hit.distance / car.GetComponent<Rigidbody2D>().velocity.magnitude;

                                    float oppAccel = oppScript.accelerationAdj;
                                    float myAccelFree = carScript.accelerationFree;

                                    

                                    // vf^2 = vi^2 + 2ad
                                    

                                    
                                    if (myTime + leftGreenGap > oppTime)
                                    {
                                        // Special case to prevent deadlock
                                        // If the other is stopped and you are moving slower than them (have spent more time there), go
                                        if (!(oppCar.GetComponent<Rigidbody2D>().velocity.magnitude < 0.2 && hit.distance < oppHit.distance))
                                        {
                                            canTurn = false;
                                            Debug.Log(oppTime + " - " + myTime);
                                        }
                                        
                                    }
                                }
                            }

                            if (canTurn) { carScript.ignoreBarriers(); }
                            else { carScript.recognizeBarriers(); }

                        }
                        else { carScript.ignoreBarriers(); } // Ignore barriers if there are no cars in the opposite lane
                    }
                    else { carScript.ignoreBarriers(); } // Ignore barriers on a green or acceptable yellow if not turning left
                }
                else if (carScript.turnSignal == 1)
                {
                    bool safeRight = true;

                    // Must be stopped at red
                    if (hit.rigidbody.velocity.magnitude > 0.4 || hit.distance > lightExtents * 2.5)
                    {
                        safeRight = false;
                    }

                    // Lane from left cannot have cars within gap going straight
                    else if (rangerScripts[(i + 3)%4].numHits > 0 
                        && rangerScripts[(i + 3) % 4].hitList[0].rigidbody.velocity.magnitude * rightRedGap > rangerScripts[(i + 3) % 4].hitList[0].distance - lightExtents - barrierHeight
                        && rangerScripts[(i + 3) % 4].hitList[0].collider.gameObject.GetComponent<MoveRight>().turnSignal == 0)
                    {
                        Debug.Log("Reason 2");
                        safeRight = false;
                    }

                    // Opposite lane cannot have cars within gap turning left
                    else if (rangerScripts[(i + 2) % 4].numHits > 0 
                        && rangerScripts[(i + 2) % 4].hitList[0].rigidbody.velocity.magnitude * rightRedGap > rangerScripts[(i + 2) % 4].hitList[0].distance - lightExtents - barrierHeight
                        && rangerScripts[(i + 2) % 4].hitList[0].collider.gameObject.GetComponent<MoveRight>().turnSignal == -1)
                    {

                        Debug.Log("Reason 3");
                        safeRight = false;
                    }

                    if (safeRight || carScript.hasRedClearance)
                    {
                        carScript.hasRedClearance = true;
                        carScript.ignoreBarriers();
                        
                    }
                    else
                    {
                        carScript.recognizeBarriers();
                    }
                    
                }
                else { carScript.recognizeBarriers(); } // Respect barriers on a red or unacceptable yellow
            }
        }
    }

    public IEnumerator switchTo(int[] orientation, float waitTime = 0, bool isYellow=false)
    {
        yield return new WaitForSeconds(waitTime);

        if (isYellow)
        {
            // Tell cars that will make it through the yellow in 3 seconds at current velocity to ignore the barrier 
            for (int i = 0; i < orientation.Length; i++)
            {
                if (orientation[i] == 2)
                {
                    foreach (RaycastHit2D hit in multiRangers[i].GetComponent<MultipleSensorScript>().hitList)
                    {
                        // If they will reach the ranger (halfway through the intersection)
                        hit.collider.GetComponent<MoveRight>().hasYellowClearance = (hit.rigidbody.velocity.magnitude * 3 > hit.distance);
                    }
                }
            }

            // Remove leftClearance whenever a light becomes red?

        }

        for (int i = 0; i < orientation.Length; i++)
        {
            barrierScripts[i].makeColor(orientation[i] - ((orientation[i] == 2 && isYellow) ? 1 : 0));

            if (orientation[i] == 0)
            {
                foreach (RaycastHit2D hit in rangerScripts[i].GetComponent<MultipleSensorScript>().hitList)
                {
                    hit.collider.GetComponent<MoveRight>().hasLeftClearance = false;
                }
            }
        }
        currentOrientation = orientation;
    }

    private void OnApplicationQuit()
    {
        isRunning = false;
    }

    public IEnumerator waitToStartCycle(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        startCycle();
    }

    public void startCycle()
    {
        if (!isRunning) { return; }
        StartCoroutine(switchTo(orientations[1]));
        StartCoroutine(switchTo(orientations[1], 10, true));
        StartCoroutine(switchTo(orientations[0], 13));
        StartCoroutine(switchTo(orientations[2], 15, false));
        StartCoroutine(switchTo(orientations[2], 25, true));
        StartCoroutine(switchTo(orientations[0], 28));
        StartCoroutine(waitToStartCycle(30));
    }
}
