﻿using UnityEngine;
using System.Collections;

public class playerShootController : MonoBehaviour {

    public bool Finished;

    private playerShooter shooter;
    public int bankedCoins;

    public RenderTexture camTexture;

    public string Name;

	// Use this for initialization
	void Start () {
        shooter = GetComponent<playerShooter>();

        if (GetComponent<NetworkView>().isMine) {
            Camera.main.transform.parent = shooter.goRing.transform;
            Camera.main.transform.localPosition = new Vector3(1.57f, 0, -3.12f);
        }
	}
	
	// Update is called once per frame
	void Update () {
        Finished = bankedCoins >= 500;

        if (GetComponent<NetworkView>().isMine) {
            UpdateMovement();
        }
	}

    public void UpdateMovement() {
        float rAxisX = Input.GetAxis("360_RightThumbstickX");
        float rAxisY = Input.GetAxis("360_RightThumbstickY");

        //Debug.Log("AXIS: " + lAxisX + ", " + rAxisX + ", " + rAxisY);
        shooter.UpdateMovement(rAxisX, rAxisY);

        float rTrigger = Input.GetAxis("360_Triggers");
        shooter.Shoot(rTrigger);
    }

    public void AddCoins(int coins)
    {
        //GetComponent<NetworkView>().RPC("AddCoinsToShooter", )
        GetComponent<NetworkView>().RPC("AddCoinsToShooter", RPCMode.AllBuffered, coins);
    }

    [RPC]
    private void AddCoinsToShooter(int coins) {
        bankedCoins += coins;
    }

    public int GetScore()
    {
        return bankedCoins;
    }

    public void SetName(string newName) {
        GetComponent<NetworkView>().RPC("SetClientNames", RPCMode.AllBuffered, newName);
    }

    [RPC]
    private void SetClientNames(string newName) {
        Name = newName;
    }
}
