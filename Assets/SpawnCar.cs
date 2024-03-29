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
    public float spawnPct = 1;
    public float leftPct = 0.25f;
    public float rightPct = 0.25f;


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
            timer = 0;

            float rand = Random.value;

            if (rand < spawnPct)
            {
                GameObject newCar = Instantiate(car, transform.position, this.transform.rotation);
                var carScript = newCar.GetComponent<MoveRight>();
                var roadScript = road.GetComponent<RoadInfo>();

                carScript.speedLimit = roadScript.speedLimit;

                rand = Random.value;

                if (rand < rightPct)
                {
                    carScript.turnSignal = 1;
                }
                else if (rand > 1 - leftPct)
                {
                    carScript.turnSignal = -1;
                }
                else { carScript.turnSignal = 0;}

            }
        }
    }
}
