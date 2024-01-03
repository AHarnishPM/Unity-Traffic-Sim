using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class StandardLightScript : MonoBehaviour
{
    public GameObject[] barriers = new GameObject[4];

    public GameObject[] multiRangers = new GameObject[4];

    public LightBarrierScript[] scripts = new LightBarrierScript[4];

    public int[] currentOrientation = {0, 0, 0, 0};

    private float timer = 0;

    private bool isPausing = false;

    private bool isTransitioning = false;

    // 0 = red, 1=yellow, 2=green
    // Values in this order: top, left, right, bottom
    // When transitioning, must approach first orientation where all are red first
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
    // Start is called before the first frame update
    void Start()
    {
        for(int count = 0; count < barriers.Length; count++)
        {
            scripts[count] = barriers[count].GetComponent<LightBarrierScript>();
        }
        safeTransitionTo(orientations[1]);
        safeTransitionTo(orientations[2]);

    }

    // Update is called once per frame
    void Update()
    {
        // Calculate yellow time, for now default 3 seconds
        timer += Time.deltaTime;
        if (timer >= 3 && isTransitioning)
        {
            switchTo(orientations[0]);
            isTransitioning = false;
            isPausing = true;
            timer = 0;
        }

        if (isPausing && timer >= 1)
        {
            isPausing = false;
            switchTo(currentOrientation);
        }
    }

    public void switchTo(int[] orientation)
    {
        for (int i = 0; i < orientation.Length; i++)
        {
            scripts[i].makeColor(orientation[i]);
        }
    }

    public void safeTransitionTo(int[] orientation)
    {
        if (isTransitioning)
        {
            throw new System.Exception("Cannot change orientation while already changing");
        }

        int[] transOrientation = new int[orientation.Length];
        for (int i = 0; i < orientation.Length; i++)
        {
            if (orientation[i] == 0 && currentOrientation[i] == 2)
            {
                isTransitioning = true;
                timer = 0;
                transOrientation[i] = 1;
            }
            else
            {
                transOrientation[i] = orientation[i];
            }
        }
        currentOrientation = orientation;

        switchTo(transOrientation);
    }
}
