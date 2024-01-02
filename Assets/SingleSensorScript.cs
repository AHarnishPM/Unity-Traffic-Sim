using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSensorScript : MonoBehaviour
{
    public RaycastHit2D hit;
    public LayerMask mask;
    public float viewDistance = 1000f;

    // Start is called before the first frame update
    void Start()
    {
        hit = Physics2D.Raycast(transform.position, transform.rotation * Vector2.right, viewDistance, mask);
    }

    // Update is called once per frame
    void Update()
    {
        hit = Physics2D.Raycast(transform.position, transform.rotation * Vector2.right, 1000f, mask);
    }
}
