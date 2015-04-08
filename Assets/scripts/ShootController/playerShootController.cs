using UnityEngine;
using System.Collections;

public class playerShootController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        shooter = GetComponent<playerShooter>();

        if (GetComponent<NetworkView>().isMine) {
            Camera.main.transform.parent = transform;

            Camera.main.transform.localposition = new Vector3(0, 1, -3);
        }
	}
	
	// Update is called once per frame
	void Update () {
        UpdateMovement();
	}

    public override void UpdateMovement() {

    }
}
