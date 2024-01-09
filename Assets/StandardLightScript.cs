using System;
using System.Collections;
using System.Collections.Generic;
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
        new int[] { 0, 2, 2, 0 }, // Horizontal free, vertical blocked
        new int[] { 2, 0, 0, 2 }, // Vertical free, horizontal blocked
        new int[] { 2, 0, 0, 0 }, // Top open
        new int[] { 0, 2, 0, 0 }, // Left open
        new int[] { 0, 0, 2, 0 }, // Right open
        new int[] { 0, 0, 0, 2 }  // Bottom open
    };

    private bool isRunning;

    Queue<Action> jobs = new Queue<Action>();

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
        // If this car is trying to turn left and it would be unsafe to do so, tell it to not ignore barriers.
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
                        // If they will reach the ranger (halfway through the intersectio)
                        if (hit.rigidbody.velocity.magnitude * 3 > hit.distance)
                        {
                            hit.collider.GetComponent<MoveRight>().ignoreBarriers();
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
