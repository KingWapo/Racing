using UnityEngine;
using System.Collections;

public class playerShootController : shootController {

	// Use this for initialization
	void Start () {
        shooter = GetComponent<playerShooter>();

        if (GetComponent<NetworkView>().isMine) {
            Camera.main.transform.parent = transform.FindChild("RailGun");

            Camera.main.transform.localPosition = new Vector3(-3, 1, 0);
            Camera.main.transform.localRotation = Quaternion.Euler(30, 90, 0);
        }
	}
	
	// Update is called once per frame
	void Update () {
        UpdateMovement();
	}

    public override void UpdateMovement() {
        shooter.UpdateMovement(Input.GetAxis("Horizontal"), 0, 0);
    }
}
