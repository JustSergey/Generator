﻿using System.Collections;
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
        cars = new Transform[transform.childCount];
        begin_positions = new Vector3[transform.childCount];
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
            for (int k = 0, i = bound; i < bound * 2; i++, k++)
            {
                Destroy(cars[i].gameObject);
                cars[i] = Instantiate(cars[k], transform);
            }

            for (int i = 0; i < bound; i++)
                cars[i].GetComponent<Generate>().Respawn(begin_positions[i], Quaternion.identity, RespawnType.Default);
            for (int i = bound; i < bound * 2; i++)
                cars[i].GetComponent<Generate>().Respawn(begin_positions[i], Quaternion.identity, RespawnType.Mutation);
            for (int i = bound * 2; i < cars.Length; i++)
                cars[i].GetComponent<Generate>().Respawn(begin_positions[i], Quaternion.identity, RespawnType.Recreate);

            time = 0f;
        }
        else
        {
            time += Time.fixedDeltaTime;
        }
    }
}
