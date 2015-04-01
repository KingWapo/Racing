using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {

    protected int waypointsHit;
    protected playerRacer racer;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    virtual public void UpdateMovement()
    {

    }

    virtual public int Score()
    {
        return waypointsHit;
    }

}
