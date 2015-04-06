using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : Controller
{
    public float Sensitivity;

    private int targetIndex;
    private NavMeshAgent agent;
    private List<GameObject> waypoints;


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

        waypoints = new List<GameObject>();
        GameObject WaypointList = GameObject.FindGameObjectWithTag("Waypoint List");
        for (int i = 0; i < WaypointList.transform.childCount; i++)
        {
            waypoints.Add(WaypointList.transform.GetChild(i).gameObject);
        }

        agent.SetDestination(waypoints[targetIndex].transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        
        print(targetIndex);
        if (other.gameObject == waypoints[targetIndex])
        {
            targetIndex = (targetIndex + 1) % waypoints.Count;
            agent.SetDestination(waypoints[targetIndex].transform.position);
            waypointsHit++;
        }
        
    }

    public override void UpdateMovement()
    {

    }

}
