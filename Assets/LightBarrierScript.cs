using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBarrierScript : MonoBehaviour
{
    public BoxCollider2D myCollider;
    public int lightStatus; // red: -1 ; yellow: 0 ; green: 1

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void makeRed()
    {
        myCollider.enabled = true;
        lightStatus = -1;
    }

    public void makeGreen()
    {
        myCollider.enabled=false;
        lightStatus = 1;
    }

    public void makeYellow()
    {
        myCollider.enabled = false;
        lightStatus=0;
    }

}
