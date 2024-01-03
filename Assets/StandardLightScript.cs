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

    public int[] currentOrientation = {0, 0, 0, 0};

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

    Queue<Action> jobs = new Queue<Action>();

    // Start is called before the first frame update
    void Start()
    {
        for(int count = 0; count < barriers.Length; count++)
        {
            scripts[count] = barriers[count].GetComponent<LightBarrierScript>();
        }

        Thread sequencerThread = new Thread(sequencer);
        sequencerThread.Start();

        
    }

    // Update is called once per frame
    void Update()
    {
        // switchTo has components that can only be called to from main thread, and I want to use Thread.Sleep to handle waiting
        // instead of messy timer loops.
        while (jobs.Count > 0)
        {
            jobs.Dequeue().Invoke();
        }
    }

    public void switchTo(int[] orientation)
    {
        for (int i = 0; i < orientation.Length; i++)
        {
            scripts[i].makeColor(orientation[i]);
        }
        currentOrientation = orientation;
    }

    // Updates traffic lights at set intervals, can repeat by calling sequencer() again at the end.
    // Plan to make this cleaner for ML integration.
    private void sequencer()
    {
        Debug.Log("YEAH");
        jobs.Enqueue(() => switchTo(orientations[3]));
        Thread.Sleep(3000);
        jobs.Enqueue(() => switchTo(orientations[1]));
    }
}
