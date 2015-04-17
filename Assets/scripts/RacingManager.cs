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
	void Update () {
        UpdatePlacement();
	}

    public void UpdatePlacement()
    {
        if (racers.Count > 0)
        {
            /*places = "Racer 1 " + racers[0].GetComponent<RacerInformation>().Place + " with " + racers[0].GetComponent<RacerInformation>().Progress() + "\n";
            for (int i = 1; i < racers.Count; i++)
            {
                if (racers[i].GetComponent<RacerInformation>().Progress() > racers[i - 1].GetComponent<RacerInformation>().Progress())
                {
                    GameObject temp = racers[i - 1];
                    racers[i - 1] = racers[i];
                    racers[i] = temp;
                    racers[i - 1].GetComponent<RacerInformation>().Place = i;
                    racers[i].GetComponent<RacerInformation>().Place = i + 1;
                }
                places += "Racer " + (i + 1) + " " + racers[i].GetComponent<RacerInformation>().Place + " with " + racers[i].GetComponent<RacerInformation>().Progress() + "\n";
            }
            //PlacementText.text = places;

            if (racers[0].GetComponent<RacerInformation>().Finished)
            {
                GetComponent<networkManager>().NextLevel();
            }*/

            for (int i = 0; i < racers.Count; i++)
            {
                if (racers[i].GetComponent<RacerInformation>().Finished)
                {
                    GetComponent<networkManager>().EndMatch();
                }
            }
        }
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
