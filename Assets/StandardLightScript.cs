using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using UnityEngine;

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

    public float leftGreenGap = 4;

    // Start is called before the first frame update
    void Start()
    {
        for (int count = 0; count < barriers.Length; count++)
        {
            barrierScripts[count] = barriers[count].GetComponent<LightBarrierScript>();
            rangerScripts[count] = multiRangers[count].GetComponent<MultipleSensorScript>();
        }

        isRunning = true;

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

                // If light is green
                if (barrierScripts[i].lightStatus == 2)
                {
                    // If the car is turning left
                    if (carScript.turnSignal == -1)
                    {
                        // Check if it is safe to do so

                        // Will car reach the intersection leftGreenGap seconds before the closest car in the opposite lane
                        
                    }
                }
            }

            
        }
    }

    public IEnumerator switchTo(int[] orientation, float waitTime = 0, bool isYellow=false)
    {
        yield return new WaitForSeconds(waitTime);

        if (isYellow )
        {
            // Tell cars that will make it through the yellow in 3 seconds at current velocity to ignore the barrier 
            for (int i = 0; i < orientation.Length; i++)
            {
                if (orientation[i] == 2)
                {
                    foreach (RaycastHit2D hit in multiRangers[i].GetComponent<MultipleSensorScript>().hitList)
                    {
                        // If they will reach the ranger (halfway through the intersection)
                        if (hit.rigidbody.velocity.magnitude * 3 > hit.distance)
                        {
                            hit.collider.GetComponent<MoveRight>().ignoreAllBarriers();
                        }
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
