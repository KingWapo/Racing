using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RacingManager : MonoBehaviour {

    private List<GameObject> racers;
    public Text PlacementText;

    // Debug
    private string places;

	// Use this for initialization
	void Start () {
        racers = new List<GameObject>();
        places = "";
	}
	
	// Update is called once per frame
    void Update()
    {
        if (racers.Count > 0 && RaceManager.State == RaceState.Occuring)
        {
            for (int i = 0; i < racers.Count; i++)
            {
                if (racers[i].GetComponent<RacerInformation>().Finished)
                {
                    GetComponent<NetworkView>().RPC("EndGame", RPCMode.AllBuffered);
                }
            }
            GameObject shooter = GameObject.FindGameObjectWithTag("Shooter");
            if (shooter.GetComponent<playerShootController>().Finished)
            {
                GetComponent<NetworkView>().RPC("EndGame", RPCMode.AllBuffered);
            }
        }
    }

    [RPC]
    private void EndGame()
    {
        RaceManager.State = RaceState.End;

        List<GameObject> tempList = racers;

        int high = 0;
        int index = -1;

        for (int i = 0; i < tempList.Count; i++)
        {
            for (int j = i; j < tempList.Count; j++)
            {
                if (tempList[j].GetComponent<RacerInformation>().GetScore() > high)
                {
                    high = tempList[j].GetComponent<RacerInformation>().GetScore();
                    index = j;
                }
            }
            if (index != i)
            {
                GameObject temp = tempList[i];
                tempList[i] = tempList[index];
                tempList[index] = temp;
            }
            high = 0;
            index = -1;
        }
        
        GameObject shooter = GameObject.FindGameObjectWithTag("Shooter");

        for (int i = 0; i < tempList.Count; i++)
        {
            if (shooter.GetComponent<playerShootController>().GetScore() > tempList[i].GetComponent<RacerInformation>().GetScore())
            {
                tempList.Insert(i, shooter);
                break;
            }
            if (i == tempList.Count - 1)
            {
                tempList.Add(shooter);
                break;
            }
        }

        GetComponent<RaceManager>().SetPlayers(tempList);
    }

    public void AddRacer(GameObject racer, int startPosition)
    {
        if (!PlacementText)
        {
            print("In here");
            PlacementText = GameObject.FindGameObjectWithTag("PlacementText").GetComponent<Text>();
        }
        racer.GetComponent<RacerInformation>().Place = (startPosition + 1);
        racers.Add(racer);
    }

    public void RemoveRacer(NetworkPlayer player)
    {
        foreach(GameObject racer in racers)
        {
            if (racer.GetComponent<RacerInformation>().Player == player)
            {
                racers.Remove(racer);
            }
        }
    }
}
