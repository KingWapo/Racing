using UnityEngine;
using System.Collections;

public class playerLap : MonoBehaviour {

    public static int lap;
    public GameObject finishLine;
   
	// Use this for initialization
	void Start () {
        lap = 0;
	}
	
	// Update is called once per frame
	void Update () {
  
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == finishLine)
        {
            lap += 1;
        }
    }

}
