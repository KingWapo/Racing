using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class gameLobby : MonoBehaviour {

    public Button addOne;
    public Button addAll;
    public Button removeOne;
    public Button removeAll;
    public Button start;
    public Button leave;

	// Use this for initialization
    void Start() {
        addOne.onClick.AddListener(() => { AddOne(); });
        addAll.onClick.AddListener(() => { AddAll(); });
        removeOne.onClick.AddListener(() => { RemoveOne(); });
        removeAll.onClick.AddListener(() => { RemoveAll(); });
        start.onClick.AddListener(() => { StartGame(); });
        leave.onClick.AddListener(() => { LeaveLobby(); });
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void AddOne() {
        Debug.Log("ADD TRACK");
    }

    void AddAll() {
        Debug.Log("ADD ALL");
    }

    void RemoveOne() {
        Debug.Log("REMOVE TRACK");
    }

    void RemoveAll() {
        Debug.Log("REMOVE ALL");
    }

    void StartGame() {
        Debug.Log("START GAME");
    }

    void LeaveLobby() {
        Debug.Log("LEAVE LOBBY");
        Application.LoadLevel("MainMenu");
    }
}
