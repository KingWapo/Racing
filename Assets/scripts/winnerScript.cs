using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class winnerScript : MonoBehaviour {

    Text text;
    public GameObject player;
    public GameObject enemy001;
    public GameObject enemy002;
    public GameObject enemy003;
    public GameObject enemy004;
    public GameObject enemy005;
    public int var;
   

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        text.enabled = false;
        player.GetComponent<agentScript>().enabled = false;
        var = 1;
	}
	
	// Update is called once per frame
	void Update () {
      //  print(agentScript.enemyWinner);

        print(enemyLap.lap);
        if (var == 1)
        {
            if (playerLap.lap == 4)
            {
                if (enemyLap.lap == 4)
                {
                    text.enabled = true;
                    text.text = "Loser!";
                    var = 2;
                }
                if (enemyLap002.lap == 4)
                {
                    text.enabled = true;
                    text.text = "Loser!";
                    var = 2;

                }
                if (enemyLap003.lap == 4)
                {
                    text.enabled = true;
                    text.text = "Loser!";
                    var = 2;

                }
                if (enemyLap004.lap == 4)
                {
                    text.enabled = true;
                    text.text = "Loser!";
                    var = 2; 
                }
                if (enemyLap005.lap == 4)
                {
                    text.enabled = true;
                    text.text = "Loser!";
                    var = 2;
                }
                else
                {
                    text.enabled = true;
                    text.text = "Winner!";
                    var = 2;
                }
            }
        }
	}
}
