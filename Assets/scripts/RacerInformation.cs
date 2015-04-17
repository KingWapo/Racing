using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RacerInformation : MonoBehaviour {

    // Car information
    public string Name;
    public NetworkPlayer Player;

    // Determining placement
    public bool Finished;
    public int Place;
    public int waypointsHit;
    public GameObject previousWaypoint;

    private int bankedCoins = 0;
    private int unbankedCoins = 0;
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
        Finished = bankedCoins + unbankedCoins > 1000;
        if (GetComponent<PlayerController>())
        {
            print("Score: " + (bankedCoins + unbankedCoins));
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Waypoint")
        {
            waypointsHit++;
            previousWaypoint = other.gameObject;

        }
        if (other.tag == "FinishLine")
        {
            waypointsHit++;
            previousWaypoint = other.gameObject;

            bankedCoins += unbankedCoins;
            unbankedCoins = 0;
        }

        if (other.tag == "Coin")
        {
            unbankedCoins++;
            Destroy(other.gameObject);
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
