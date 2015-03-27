using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class countDownScript : MonoBehaviour {

    public float countDown = 1;
    public GameObject RacerParent;

    private Text text;
    private bool setStart;


	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        
        text.enabled = false;
        setStart = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time <=1)
        {
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

            if (!setStart)
            {
                for (int i = 0; i < RacerParent.transform.childCount; i++)
                {
                    if (RacerParent.transform.GetChild(i).gameObject.activeSelf)
                        RacerParent.transform.GetChild(i).gameObject.GetComponent<Racer>().HitGo();
                }
                setStart = true;
            }
        }
        else
        {
            text.enabled = false;
        }

    }
}
