using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PoliceBehaviour : MonoBehaviour
{
    public NavMeshAgent officer;
    public GameObject[] pPoints;
    Vector3 target;
    float threshold = 1f;

    void Start()
    {
        Patrol();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(officer.transform.position,target) < threshold)
        {
            Patrol();
        }
    }
    
    void Patrol()
    {
        target = pPoints[Random.Range(0, 9)].transform.position;
        officer.destination = target;
    }
}
