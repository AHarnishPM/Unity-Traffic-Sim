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
