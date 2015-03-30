using UnityEngine;
using System.Collections;

public class PlayerController : Controller
{
    public float speed = 10f;

    private Rigidbody rigidbody;
    private NavMeshAgent agent;

    private float sensitivity = .5f;

    private float playerRotation = 300f;
    private float playerLean = 0f;

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

        if (turnAxis != 0)
        {
            transform.Rotate(0, turnAxis * Time.deltaTime * playerRotation * sensitivity, 0);

            if (playerLean >= -12 && playerLean <= 12)
            {
                playerLean -= Mathf.Sign(turnAxis);
            }
        }
        else
        {
            if (playerLean > 0)
            {
                playerLean -= 1;
            }
            else if (playerLean < 0)
            {
                playerLean += 1;
            }
        }

        if (acclAxis > .1f || acclAxis < -.1f)
        {
            playerVelocity = Mathf.Max(maxReverseVel, Mathf.Min(maxForwardVel, playerVelocity + .5f * acclAxis));
        }
        else
        {
            playerVelocity *= .9f;
            if (Mathf.Abs(playerVelocity) <= .000f)
            {
                playerVelocity = 0f;
            }
        }

        transform.Rotate(playerLean, 0, 0);
        transform.Translate(Time.deltaTime * playerVelocity, 0, 0);
    }
}
