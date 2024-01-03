using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardLightScript : MonoBehaviour
{
    public GameObject barrierTop;
    public GameObject barrierLeft;
    public GameObject barrierRight;
    public GameObject barrierBottom;

    public GameObject mutliRangerTop;
    public GameObject multiRangerLeft;
    public GameObject multiRangerRight;
    public GameObject multiRangerBottom;

    public LightBarrierScript topScript;
    public LightBarrierScript leftScript;
    public LightBarrierScript rightScript;
    public LightBarrierScript bottomScript;

    // 0 = red, 1=yellow, 2=green
    // Values in this order: top, left, right, bottom
    // When transitioning, must approach first orientation where all are red first
    public int[,] orientations =
    {
        {0, 0, 0, 0 }, // All blocked
        {0, 2, 2, 0 }, // Horizontal free, vertical blocked
        {2, 0, 0, 2 }, // Vertical free, horizontal blocked
        {2, 0, 0, 0 }, // Top open
        {0, 2, 0, 0 }, // Left open
        {0, 0, 2, 0 }, // Right open
        {0, 0, 0, 2 }  // Bottom open
    };
    // Start is called before the first frame update
    void Start()
    {
        topScript = barrierTop.GetComponent<LightBarrierScript>();
        bottomScript = barrierBottom.GetComponent<LightBarrierScript>();
        leftScript = barrierLeft.GetComponent<LightBarrierScript>();
        rightScript = barrierRight.GetComponent<LightBarrierScript>();

        topScript.makeColor(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
