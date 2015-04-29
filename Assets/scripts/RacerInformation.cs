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
    public int unbankedCoins = 0;
    private Text placementText;

    private playerRacer racer;

    // Debug
    public float Score;

	// Use this for initialization
	void Start () {
        waypointsHit = 0;
        placementText = GameObject.FindGameObjectWithTag("Place").GetComponent<Text>();
        racer = GetComponent<playerRacer>();
	}
	
	// Update is called once per frame
	void Update () {
        Finished = bankedCoins > 60;
        if (GetComponent<PlayerController>())
        {
            placementText.text = "Score: " + bankedCoins;
            print("Score: " + bankedCoins);
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

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Racer")
        {
            racer.BeginSlow();
        }

        if (collision.collider.tag == "Coin")
        {
            unbankedCoins++;
            Network.Destroy(collision.collider.gameObject);
            if (GetComponent<AIController>())
            {
                
            }
        }

        if (collision.collider.tag == "Bullet") {
            GetComponent<playerRacer>().TeleportToStart();
            FindObjectOfType<playerShootController>().AddCoins(unbankedCoins);
            unbankedCoins = 0;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Racer")
        {
            racer.StopSlow();
        }
    }

    public int GetScore()
    {
        return bankedCoins;
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
