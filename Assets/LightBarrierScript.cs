using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBarrierScript : MonoBehaviour
{
    public BoxCollider2D myCollider;
    public SpriteRenderer myRenderer;
    public Sprite[] sprites = new Sprite[3];
    public int lightStatus; // red: 0 ; yellow: 1 ; green: 2

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void makeColor(int colorID)
    {
        // red: 0
        // yellow: 1
        // green: 2
        myCollider.enabled = colorID == 0;
        lightStatus = colorID;
        myRenderer.sprite = sprites[colorID];
    }

}
