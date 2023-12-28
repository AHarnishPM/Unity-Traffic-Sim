using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCar : MonoBehaviour
{
    public GameObject car;
    public float SpawnRate = 2;
    private float timer = 0;
    public float carsLeft = 2;
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
                Instantiate(car, transform.position, transform.rotation);
                carsLeft--;
            }
        }
    }
}
