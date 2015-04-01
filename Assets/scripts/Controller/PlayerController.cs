using UnityEngine;
using System.Collections;

public class PlayerController : Controller {

	// Use this for initialization
    void Start() {
        racer = GetComponent<playerRacer>();

        if (GetComponent<NetworkView>().isMine) {
            Camera.main.transform.parent = transform;

            Camera.main.transform.localPosition = new Vector3(-3, 1, 0);
            //Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(15, 90, 0));
        }
	}
	
	// Update is called once per frame
	void Update () {
        UpdateMovement();
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Waypoint")
        {
            waypointsHit++;
        }
    }

    public override void UpdateMovement()
    {
        base.UpdateMovement();
        float turnAxis = Input.GetAxis("Horizontal");
        float acclAxis = Input.GetAxis("360_Triggers");

        float playerLean = racer.UpdateMovement(turnAxis, acclAxis);

        Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(15, 90, -playerLean));
    }
}
