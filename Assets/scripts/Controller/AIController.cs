using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : Controller
{
    public float Sensitivity;

    private int targetIndex;
    private NavMeshAgent agent;
    private List<GameObject> waypoints;

    private GameObject priorityTarget;


	// Use this for initialization
    void Start() {
        racer = GetComponent<playerRacer>();

	    if (!GetComponent<NavMeshAgent>())
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
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
	    if (!priorityTarget)
        {
            SetTargetToWaypoint();
        }
	}

    void OnTriggerEnter(Collider other)
    {
        
        //print(targetIndex);
        if (other.gameObject == waypoints[targetIndex])
        {
            targetIndex = (targetIndex + 1) % waypoints.Count;
            agent.SetDestination(waypoints[targetIndex].transform.position);
            //waypointsHit++;
        }

        if (other.tag == "Coin")
        {
            if (!priorityTarget)
            {
                priorityTarget = other.gameObject;
                agent.SetDestination(priorityTarget.transform.position);
            }
        }
        
    }

    public void SetTargetToWaypoint()
    {
        agent.SetDestination(waypoints[targetIndex].transform.position);
    }

    public override void UpdateMovement()
    {

    }

}
