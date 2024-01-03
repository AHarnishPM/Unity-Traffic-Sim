using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LightSequencer : MonoBehaviour
{
    public StandardLightScript lightScript;

    // 0 = red, 1=yellow, 2=green
    // Values in this order: top, left, right, bottom
    // When transitioning, must approach first orientation where all are red first
    

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
