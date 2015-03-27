using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RacingManager : MonoBehaviour {

    public GameObject RacersParent;

    private List<GameObject> racers;

	// Use this for initialization
	void Start () {
        racers = new List<GameObject>();
	    for (int i = 0; i < RacersParent.transform.childCount; i++)
        {
            racers.Add(RacersParent.transform.GetChild(i).gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
        UpdatePlacement();
	}

    public void UpdatePlacement()
    {
        QuickSort();
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
