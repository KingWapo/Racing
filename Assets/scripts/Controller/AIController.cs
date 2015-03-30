using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIController : Controller
{
    public List<GameObject> WayPoints;

    public float Sensitivity;

    private int targetIndex;
    private NavMeshAgent agent;
    private int waypointsHit;


	// Use this for initialization
	void Start () {
	    if (!GetComponent<NavMeshAgent>())
        {
            
        }
        else
        {
            agent = GetComponent<NavMeshAgent>();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    virtual void UpdateMovement()
    {

    }

}
