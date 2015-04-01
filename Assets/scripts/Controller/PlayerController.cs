using UnityEngine;
using System.Collections;

public class PlayerController : Controller
{
    public float speed = 10f;

    private new Rigidbody rigidbody;
    private NavMeshAgent agent;

    private float sensitivity = .5f;

    private float playerRotation = 300f;
    private float playerLean = 0f;
    private float maxPlayerLean = 12f;

    private float playerVelocity = 0f;

    private float maxForwardVel = 30f;
    private float maxReverseVel = -4f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
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
        Debug.Log("trigger - " + acclAxis);

        if (Mathf.Abs(turnAxis) > .1f && Mathf.Abs(acclAxis) > .1f) {
            playerLean = Mathf.Clamp(playerLean - Mathf.Sign(turnAxis), -maxPlayerLean, maxPlayerLean);
        } else {
            if (playerLean > 0f) {
                playerLean -= 1f;
            } else if (playerLean < 0f) {
                playerLean += 1f;
            }
        }

        if (Mathf.Abs(acclAxis) > .1f) {
            if (turnAxis != 0) {
                transform.Rotate(0, turnAxis * Time.deltaTime * playerRotation * sensitivity * Mathf.Sign(acclAxis), 0);
            }

            playerVelocity = Mathf.Clamp(playerVelocity + .5f * acclAxis, maxReverseVel, maxForwardVel);
        } else {
            playerVelocity *= .9f;
            if (Mathf.Abs(playerVelocity) <= .0001f) {
                playerVelocity = 0f;
            }
        }

        transform.Rotate(playerLean, 0, 0);
        transform.Translate(Time.deltaTime * playerVelocity, 0, 0);

        Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(15, 90, -playerLean));
    }
}
