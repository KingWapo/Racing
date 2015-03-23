using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class countDownScript : MonoBehaviour {

    public float countDown = 1;
    public GameObject player;
    public GameObject enemy001;
    public GameObject enemy002;
    public GameObject enemy003;
    public GameObject enemy004;
    public GameObject enemy005;

    Text text;


	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        
        text.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time <=1)
        {
            player.GetComponent<xboxController>().enabled = false;
            enemy001.GetComponent<agentScript>().enabled = false;
            enemy002.GetComponent<agentScript>().enabled = false;
            enemy003.GetComponent<agentScript>().enabled = false;
            enemy004.GetComponent<agentScript>().enabled = false;
            enemy005.GetComponent<agentScript>().enabled = false;
            


            text.enabled = false;
        }
        else if(Time.time <=2)
        {
            text.enabled = true;
            text.text = "1";
        }
        else if(Time.time <=3)
        {
            text.text = "2";
        }
        else if(Time.time <=4)
        {
            text.text = "3";
        }
        else if(Time.time <=5)
        {
            text.text = "GO!";

            enemy001.GetComponent<agentScript>().target = 2;
            enemy002.GetComponent<agentScript>().target = 2;
            enemy003.GetComponent<agentScript>().target = 2;
            enemy004.GetComponent<agentScript>().target = 2;
            enemy005.GetComponent<agentScript>().target = 2;
            player.GetComponent<xboxController>().enabled = true;
            enemy001.GetComponent<agentScript>().enabled = true;
            enemy002.GetComponent<agentScript>().enabled = true;
            enemy003.GetComponent<agentScript>().enabled = true;
            enemy004.GetComponent<agentScript>().enabled = true;
            enemy005.GetComponent<agentScript>().enabled = true;
        }
        else
        {
            text.enabled = false;
        }

    }
}
