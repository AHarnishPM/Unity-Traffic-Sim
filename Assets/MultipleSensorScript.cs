using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleSensorScript : MonoBehaviour
{
    public List<RaycastHit2D> hitList = new();
    public ContactFilter2D contactFilter;
    public float viewDistance = 1000f;
    public int numHits;

    // Start is called before the first frame update
    void Start()
    {
        numHits = Physics2D.Raycast(transform.position, transform.rotation * Vector2.right, contactFilter, hitList, viewDistance);
    }

    // Update is called once per frame
    void Update()
    {
        numHits = Physics2D.Raycast(transform.position, transform.rotation * Vector2.right, contactFilter, hitList, viewDistance);
    }
}
