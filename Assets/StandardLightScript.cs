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
                MoveRight carScript = hit.collider.gameObject.GetComponent<MoveRight>();

                // If light is green or would make it through on yellow
                if (barrierScripts[i].lightStatus == 2 || (barrierScripts[i].lightStatus == 1 && carScript.hasYellowClearance))
                {
                    // If the car is turning left
                    if (carScript.turnSignal == -1)
                    {
                        // If there are cars in the opposite lane
                        if (rangerScripts[(i+2)%4].numHits > 0)
                        {
                            bool safeLeft = true;

                            var closestHit = rangerScripts[(i + 2) % 4].hitList[0];

                            float myTime = hit.distance / hit.rigidbody.velocity.magnitude;
                            float otherTime = closestHit.distance / closestHit.rigidbody.velocity.magnitude;

                            // If the other car is turning left it's okay
                            int otherTurn = closestHit.collider.gameObject.GetComponent<MoveRight>().turnSignal;

                            if (myTime + leftGreenGap > otherTime && otherTurn != -1)
                            {
                                safeLeft = false;
                            }

                            if (safeLeft)
                            {
                                carScript.ignoreBarriers();
                            }
                            else
                            {
                                carScript.recognizeBarriers();
                            }
                        }
                        else { carScript.ignoreBarriers(); } // Ignore barriers if there are no cars in the opposite lane
                    }
                    else { carScript.ignoreBarriers(); } // Ignore barriers on a green or acceptable yellow if not turning left
                }
                else if (carScript.turnSignal == 1 && !carScript.hasRedClearance)
                {
                    bool safeRight = true;

                    // Must be stopped at red
                    if (hit.rigidbody.velocity.magnitude > 0.4 || hit.distance > lightExtents * 1.5)
                    {
                        safeRight = false;
                    }

                    // Lane from left cannot have cars within gap going straight
                    else if (rangerScripts[(i + 3)%4].numHits > 0 
                        && rangerScripts[(i + 3) % 4].hitList[0].rigidbody.velocity.magnitude * rightRedGap > rangerScripts[(i + 3) % 4].hitList[0].distance
                        && rangerScripts[(i + 3) % 4].hitList[0].collider.gameObject.GetComponent<MoveRight>().turnSignal == 0)
                    {

                        safeRight = false;
                    }

                    // Opposite lane cannot have cars within gap turning left
                    else if (rangerScripts[(i + 2) % 4].numHits > 0 
                        && rangerScripts[(i + 2) % 4].hitList[0].rigidbody.velocity.magnitude * rightRedGap > rangerScripts[(i + 2) % 4].hitList[0].distance
                        && rangerScripts[(i + 2) % 4].hitList[0].collider.gameObject.GetComponent<MoveRight>().turnSignal == -1)
                    {


                        safeRight = false;
                    }

                    if (safeRight)
                    {
                        Debug.Log("Right on red");
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
        }

        for (int i = 0; i < orientation.Length; i++)
        {
            barrierScripts[i].makeColor(orientation[i] - ((orientation[i] == 2 && isYellow) ? 1 : 0));
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
        StartCoroutine(switchTo(orientations[1], 3, true));
        StartCoroutine(switchTo(orientations[0], 6));
        StartCoroutine(switchTo(orientations[2], 7, false));
        StartCoroutine(switchTo(orientations[2], 10, true));
        StartCoroutine(switchTo(orientations[0], 13));
        StartCoroutine(waitToStartCycle(14));
    }
}
