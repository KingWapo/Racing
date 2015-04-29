using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Countdown : MonoBehaviour {

    private float time = 0.0f;
    private Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;

        if (time < 1)
        {
            text.text = "3";
        }
        else if (time < 2)
        {
            text.text = "2";
        }
        else if (time < 3)
        {
            text.text = "1";
        }
        else if (time < 4)
        {
            text.text = "GO!";
            RaceManager.State = RaceState.Occuring;
        }
        else
        {
            text.text = "";
            enabled = false;
        }
	}
}
