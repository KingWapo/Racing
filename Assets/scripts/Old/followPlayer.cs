using UnityEngine;
using System.Collections;

public class followPlayer : MonoBehaviour {

    public GameObject player;
    
	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetAxis("Horizontal") > 0)
        {
            //print(Input.GetAxis("Horizontal"));
            //transform.Rotate(0, Input.GetAxis("Horizontal") * Time.deltaTime * player.playerRotation, 0);
            if (Input.GetAxis("360_Triggers") > 0.1)
            {
                print("Lean to the Right!");
                transform.Rotate(0, 0, 5);
                if(transform.rotation.z == 5)
                {
                    transform.Rotate(0, 0, 0);
                }

            }
            if (Input.GetAxis("360_Triggers") < 0.1)
            {
                print("bring it back to the left");
                transform.Rotate(0, 0, -2);
                if (transform.rotation.z == -2)
                {
                    transform.Rotate(0, 0, 0);
                }
            }
        }

	}
}
