﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum RaceState { Begin, Occuring, End }

public class RaceManager : MonoBehaviour {

    public static RaceState State;

    private GameObject endScreenPanel;

	// Use this for initialization
	void Start () {
        State = RaceState.Begin;
	}
	
	// Update is called once per frame
	void Update () {
        if (!endScreenPanel && State == RaceState.Occuring)
        {
            endScreenPanel = GameObject.FindGameObjectWithTag("EndScreen");
        }
	}

    public void SetPlayers(List<GameObject> players)
    {
        GetComponent<NetworkView>().RPC("SetPlayersRPC", RPCMode.AllBuffered, players);
    }

    [RPC]
    private void SetPlayersRPC(List<GameObject> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            Text txt = endScreenPanel.transform.GetChild(i).gameObject.GetComponent<Text>();

            if (players[i].GetComponent<RacerInformation>())
            {
                txt.text = players[i].GetComponent<RacerInformation>().Name + ": " + players[i].GetComponent<RacerInformation>().GetScore() + " -- Racer";
            }
            else
            {
                txt.text = players[i].GetComponent<playerShootController>().Name + ": " + players[i].GetComponent<playerShootController>().GetScore() + " -- Shooter";
            }
        }
    }

}
