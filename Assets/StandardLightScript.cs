using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class StandardLightScript : MonoBehaviour
{
    public GameObject[] barriers = new GameObject[4];

    public GameObject[] multiRangers = new GameObject[4];

    public LightBarrierScript[] scripts = new LightBarrierScript[4];

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
            scripts[count] = barriers[count].GetComponent<LightBarrierScript>();
        }

        isRunning = true;

        startCycle();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator switchTo(int[] orientation, float waitTime = 0, bool isYellow=false)
    {
        yield return new WaitForSeconds(waitTime);
        for (int i = 0; i < orientation.Length; i++)
        {
            if (orientation[i] == 2 && isYellow)
            {
                orientation[i] = 1;
            }
            scripts[i].makeColor(orientation[i]);
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
        StartCoroutine(waitToStartCycle(9));
    }
}
