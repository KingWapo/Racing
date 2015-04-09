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
        float lAxisX = Input.GetAxis("360_LeftThumbstick");
        float rAxisX = Input.GetAxis("360_RightThumbstickX");
        float rAxisY = Input.GetAxis("360_RightThumbstickY");

        Debug.Log("AXIS: " + lAxisX + ", " + rAxisX + ", " + rAxisY);
        shooter.UpdateMovement(lAxisX, rAxisX, rAxisY);
    }
}
