using UnityEngine;
using System.Collections;

public enum RaceState { Begin, Occuring, End }

public class RaceManager : MonoBehaviour {

    public static RaceState State;

	// Use this for initialization
	void Start () {
        State = RaceState.Begin;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
