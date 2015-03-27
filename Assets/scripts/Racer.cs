using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Racer : MonoBehaviour {

    public bool isAI;
    public List<GameObject> WayPoints;
    //public List<Transform> CheckPoints;

    public float Sensitivity;

    private int targetIndex;
    private NavMeshAgent agent;
    private int waypointsHit;

    // Player movement information
    private bool canGo;
    private float playerAccel = 0;
    private float playerReverse;
    private float playerRotation = 300;
    private float lean = 0;

	// Use this for initialization
	void Start () {
        targetIndex = 0;
        agent = GetComponent<NavMeshAgent>();
        waypointsHit = 0;
        canGo = false;
	}
	
	// Update is called once per frame
	void Update () {
        
        if (!isAI && canGo)
        {
            UpdateInput();
        }

	}

    void OnTriggerEnter(Collider other)
    {
        print(targetIndex);
        if (other.gameObject == WayPoints[targetIndex])
        {
            if (isAI)
            {
                targetIndex = (targetIndex + 1) % WayPoints.Count;
                agent.SetDestination(WayPoints[targetIndex].transform.position);
            }
            waypointsHit++;
        }
    }

    public void HitGo()
    {
        if (isAI)
        {
            agent.SetDestination(WayPoints[targetIndex].transform.position);
        }
        else
        {
            canGo = true;
        }
    }

    public int Score()
    {
        return waypointsHit;
    }

    private void UpdateInput()
    {
        // Steering the car left and right based on Horizontal axis
        if (Input.GetAxis("Horizontal") != 0)
        {
            transform.Rotate(0, Input.GetAxis("Horizontal") * Time.deltaTime * playerRotation * Sensitivity, 0);
            if (playerAccel > 0)
            {
                if (lean >= -12 && Input.GetAxis("Horizontal") > 0)
                {
                    transform.Rotate(0, 0, lean);
                    lean -= 1;
                }
                else if (lean <= 12 && Input.GetAxis("Horizontal") < 0)
                {
                    transform.Rotate(0, 0, lean);
                    lean += 1;
                }
                else
                {
                    transform.Rotate(0, 0, lean);
                }
            }
        }
        else
        {
            if (lean > 0)
            {
                lean -= 1;
                transform.Rotate(0, 0, lean);
            }
            else if (lean < 0)
            {
                lean += 1;
                transform.Rotate(0, 0, lean);
            }
            else if (lean == 0)
            {
                transform.Rotate(0, 0, lean);
            }
        }

        // Movement forward and in reverse based on xbox triggers
        if (Input.GetAxis("360_Triggers") > 0.1) // Right Trigger - Accelerate 
        {
            if (playerAccel < 30)
            {
                transform.Translate(0, 0, Input.GetAxis("360_Triggers") * Time.deltaTime * playerAccel);
                playerAccel += .5f;
            }
            if (playerAccel >= 30)
            {
                transform.Translate(0, 0, Input.GetAxis("360_Triggers") * Time.deltaTime * playerAccel);
            }
        }
        else if (Input.GetAxis("360_Triggers") < -0.1) // Left Trigger - Reverse
        {
            transform.Translate(0, 0, Time.deltaTime * playerAccel);
            playerAccel -= .25f;
            if (playerAccel <= 0)
            {
                playerAccel = 0;
                if (Input.GetAxis("360_Triggers") < -0.1)
                {
                    if (playerReverse < 4)
                    {
                        transform.Translate(0, 0, Input.GetAxis("360_Triggers") * Time.deltaTime * playerReverse);
                        playerReverse += .5f;
                    }
                    else if (playerReverse >= 4)
                    {
                        transform.Translate(0, 0, Input.GetAxis("360_Triggers") * Time.deltaTime * playerReverse);
                    }
                }
                else
                {
                    playerReverse = 0;
                }
            }

        }
        else // Slowing down
        {
            transform.Translate(0, 0, Time.deltaTime * playerAccel);
            playerAccel -= .025f;
            if (playerAccel <= 0)
            {
                playerAccel = 0;
            }
        }
    }
}
