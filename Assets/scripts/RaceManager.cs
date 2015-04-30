using UnityEngine;
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
        string playerScores = "";

        for (int i = 0; i < players.Count; i++) {
            if (players[i].GetComponent<RacerInformation>()) {
                playerScores += players[i].GetComponent<RacerInformation>().Name + ": " + players[i].GetComponent<RacerInformation>().GetScore() + " -- Racer";
            } else {
                playerScores += players[i].GetComponent<playerShootController>().Name + ": " + players[i].GetComponent<playerShootController>().GetScore() + " -- Shooter";
            }

            if (i < players.Count - 1) {
                playerScores += ",";
            }
        }

        GetComponent<NetworkView>().RPC("SetPlayersRPC", RPCMode.AllBuffered, playerScores);
    }

    [RPC]
    private void SetPlayersRPC(string playerScores)
    {
        string[] players = playerScores.Split(',');

        for (int i = 0; i < players.Length; i++)
        {
            Text txt = endScreenPanel.transform.GetChild(i).gameObject.GetComponent<Text>();

            txt.text = players[i];
        }
    }

}
