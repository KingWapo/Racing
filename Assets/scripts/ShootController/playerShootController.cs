using UnityEngine;
using System.Collections;

public class playerShootController : MonoBehaviour {

    private playerShooter shooter;
    private int bankedCoins;

	// Use this for initialization
	void Start () {
        shooter = GetComponent<playerShooter>();

        if (GetComponent<NetworkView>().isMine) {
            Camera.main.transform.parent = transform.FindChild("RailGun").FindChild("Gun");

            Camera.main.transform.localPosition = new Vector3(-.41f, 1.1f, 1.06f);
            Camera.main.transform.localRotation = Quaternion.Euler(0, 90, 0);
            //Camera.main.transform.localPosition = new Vector3(-1, 3, 0);
            //Camera.main.transform.localRotation = Quaternion.Euler(60, 90, 0);
        }
	}
	
	// Update is called once per frame
	void Update () {
        UpdateMovement();
	}

    public void UpdateMovement() {
        float lAxisX = Input.GetAxis("360_LeftThumbstick");
        float rAxisX = Input.GetAxis("360_RightThumbstickX");
        float rAxisY = Input.GetAxis("360_RightThumbstickY");

        //Debug.Log("AXIS: " + lAxisX + ", " + rAxisX + ", " + rAxisY);
        shooter.UpdateMovement(lAxisX, rAxisX, rAxisY);

        float rTrigger = Input.GetAxis("360_Triggers");
        shooter.Shoot(rTrigger);
    }

    public void AddCoins(int coins)
    {
        bankedCoins += coins;
    }
}
