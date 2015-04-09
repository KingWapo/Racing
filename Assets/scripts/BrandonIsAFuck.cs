using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BrandonIsAFuck : MonoBehaviour {

    // Determining placement
    public bool Finished;
    public int Place;
    public int waypointsHit;
    public GameObject previousWaypoint;

    private Text placementText;

    // Debug
    public float Score;

	// Use this for initialization
	void Start () {
        waypointsHit = 0;
        placementText = GameObject.FindGameObjectWithTag("Place").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<NetworkView>().isMine)
        {
            placementText.text = "Current Place: " + Place;
        }

        Finished = waypointsHit >= 54;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Waypoint")
        {
            waypointsHit++;
            previousWaypoint = other.gameObject;
        }

    }

    public float Progress()
    {
        if (previousWaypoint)
        {
            Score = waypointsHit * 100 + Vector3.Distance(transform.position, previousWaypoint.transform.position);
            return Score;
        }
        else
            return 1.0f / Place;
    }
}
