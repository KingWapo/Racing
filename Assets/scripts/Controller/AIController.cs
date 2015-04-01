using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : Controller
{
    public List<GameObject> WayPoints;

    public float Sensitivity;

    private int targetIndex;
    private NavMeshAgent agent;


	// Use this for initialization
    void Start() {
        racer = GetComponent<playerRacer>();

	    if (!GetComponent<NavMeshAgent>())
        {
            agent = gameObject.AddComponent<NavMeshAgent>();

            // Current values. Possibly change to being passed in.
            agent.radius = 0.75f;
            agent.speed = 62;
            agent.acceleration = 150;
        }
        else
        {
            agent = GetComponent<NavMeshAgent>();
        }

        targetIndex = 0;
        waypointsHit = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        /*
        print(targetIndex);
        if (other.gameObject == WayPoints[targetIndex])
        {
            targetIndex = (targetIndex + 1) % WayPoints.Count;
            agent.SetDestination(WayPoints[targetIndex].transform.position);
            waypointsHit++;
        }
        */
    }

    public override void UpdateMovement()
    {

    }

}
