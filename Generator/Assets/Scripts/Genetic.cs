using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genetic : MonoBehaviour
{
    [SerializeField]
    private float secondsToLife;
    private Transform[] cars;
    private Vector3[] begin_positions;
    private float time;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            cars[i] = transform.GetChild(i);
            begin_positions[i] = cars[i].position;
        }
        time = 0f;
    }
    
    void FixedUpdate()
    {
        if (time >= secondsToLife)
        {
            float[] distances = new float[cars.Length];
            for (int i = 0; i < cars.Length; i++)
                distances[i] = (cars[i].position - begin_positions[i]).magnitude;
            
            System.Array.Sort(distances, cars);

            int bound = (int)(cars.Length * 0.25f);
            int k = 0;
            for (int i = bound; i < bound * 2; i++, k++)
                cars[i] = Instantiate(cars[k]);

            for (int i = 0; i < bound; i++)
                cars[i].GetComponent<Generate>().Respawn(begin_positions[i], RespawnType.Mutation);

            time = 0f;
        }
        else
        {
            time += Time.fixedDeltaTime;
        }
    }
}
