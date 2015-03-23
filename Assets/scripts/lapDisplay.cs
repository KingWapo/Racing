using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class lapDisplay : MonoBehaviour {

    Text text;

   // public playerLap player;



    public float playerLapDisplay;
    public float enemyLap;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();

      //  player = GetComponent<playerLap>();
        

        text.text = "0/3";
	}
	
	// Update is called once per frame
	void Update () {

        if(playerLap.lap == 1)
        {
            text.text = "1/3";
        }
        else if(playerLap.lap == 2)
        {
            text.text = "2/3";
        }
        else if(playerLap.lap ==3)
        {
            text.text = "3/3";
        }
        else if(playerLap.lap >= 4)
        {
            text.enabled = false;
        }

	}
    
}
