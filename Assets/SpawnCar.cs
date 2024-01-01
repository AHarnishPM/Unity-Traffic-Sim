using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCar : MonoBehaviour
{
    public GameObject car;
    public GameObject road;
    public float SpawnRate = 5;
    private float timer = 0;
    public float carsLeft = 4;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timer < SpawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if (carsLeft > 0)
            {
                timer = 0;
                GameObject newCar = Instantiate(car, transform.position, transform.rotation);
                var carScript = newCar.GetComponent<MoveRight>();
                var roadScript = road.GetComponent<RoadInfo>();

                carScript.speedLimit = roadScript.speedLimit;
                carsLeft--;
            }
        }
    }
}
