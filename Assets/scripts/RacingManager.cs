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
            places = "Racer 1 " + racers[0].GetComponent<BrandonIsAFuck>().Place + " with " + racers[0].GetComponent<BrandonIsAFuck>().Progress() + "\n";
            for (int i = 1; i < racers.Count; i++)
            {
                if (racers[i].GetComponent<BrandonIsAFuck>().Progress() > racers[i - 1].GetComponent<BrandonIsAFuck>().Progress())
                {
                    GameObject temp = racers[i - 1];
                    racers[i - 1] = racers[i];
                    racers[i] = temp;
                    racers[i - 1].GetComponent<BrandonIsAFuck>().Place = i;
                    racers[i].GetComponent<BrandonIsAFuck>().Place = i + 1;
                }
                places += "Racer " + (i + 1) + " " + racers[i].GetComponent<BrandonIsAFuck>().Place + " with " + racers[i].GetComponent<BrandonIsAFuck>().Progress() + "\n";
            }
            PlacementText.text = places;
        }
    }

    public void AddRacer(GameObject racer, int startPosition)
    {
        if (!PlacementText)
        {
            print("In here");
            PlacementText = GameObject.FindGameObjectWithTag("PlacementText").GetComponent<Text>();
        }
        racer.GetComponent<BrandonIsAFuck>().Place = (startPosition + 1);
        racers.Add(racer);
    }

    private void QuickSort()
    {
        int left = 0;
        int right = racers.Count;
        int pivot = racers[(left + right) / 2].GetComponent<Racer>().Score();
        GameObject temp;
        // Partition
        while (left <= right)
        {
            int leftScore = racers[left].GetComponent<Racer>().Score();
            int rightScore = racers[right].GetComponent<Racer>().Score();
            while (leftScore < pivot)
            {
                left++;
            }
            while(rightScore > pivot)
            {
                right--;
            }

            if (left <= right)
            {
                temp = racers[left];
                racers[left] = racers[right];
                racers[right] = temp;
                left++;
                right--;
            }
        }
    }
}
