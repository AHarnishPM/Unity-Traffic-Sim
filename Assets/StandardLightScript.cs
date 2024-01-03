using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardLightScript : MonoBehaviour
{
    public GameObject[] barriers = new GameObject[4];

    public GameObject[] multiRangers = new GameObject[4];

    public LightBarrierScript[] scripts = new LightBarrierScript[4];

    public int currentOrientationIndex = 0;

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

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void transitionTo(int[] orientation)
    {
        for (int i = 0; i < orientation.Length; i++)
        {
            scripts[i].makeColor(orientation[i]);
        }
    }
}
