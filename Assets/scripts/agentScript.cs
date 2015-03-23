using UnityEngine;
using System.Collections;

public class agentScript : MonoBehaviour {

    public float target;
    public Transform zone001;
    public Transform zone002;
    public Transform zone003;
    public Transform zone004;
    public Transform zone005;
    public Transform zone006;
    public Transform zone007;
    public Transform zone008;
    public Transform zone009;
    public GameObject checkPoint001;
    public GameObject checkPoint002;
    public GameObject checkPoint003;
    public GameObject checkPoint004;
    public GameObject checkPoint005;
    public GameObject checkPoint006;
    public GameObject checkPoint007;
    public GameObject checkPoint008;
    public GameObject checkPoint009;
    NavMeshAgent agent;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        target = 0;
	}
	
	// Update is called once per frame
	void Update() {
        print(target);
     
        if (target == 1)
        {
            agent.SetDestination(zone001.position);
        }
        else if(target == 2)
        {
            agent.SetDestination(zone002.position);
        }
        else if(target == 3)
        {
            agent.SetDestination(zone003.position);
        }
        else if(target == 4)
        {
            agent.SetDestination(zone004.position);
        }
        else if (target == 5)
        {
            agent.SetDestination(zone005.position);
        }
        else if (target == 6)
        {
            agent.SetDestination(zone006.position);
        }
        else if (target == 7)
        {
            agent.SetDestination(zone007.position);
        }
        else if (target == 8)
        {
            agent.SetDestination(zone008.position);
        }
        else if (target == 9)
        {
            agent.SetDestination(zone009.position);
        }
    

	}
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == checkPoint001)
        {
            target = 2;
        }
        if(other.gameObject == checkPoint002)
        {
            target = 3;
        }
        if (other.gameObject == checkPoint003)
        {
            target = 4;
        }
        if(other.gameObject == checkPoint004)
        {
            target = 5;
        }
        if (other.gameObject == checkPoint005)
        {
            target = 6;
        }
        if (other.gameObject == checkPoint006)
        {
            target = 7;
        }
        if (other.gameObject == checkPoint007)
        {
            target = 8;
        }
        if (other.gameObject == checkPoint008)
        {
            target = 9;
        }
        if(other.gameObject == checkPoint009)
        {
            target = 1;
        }
        
    }
}
