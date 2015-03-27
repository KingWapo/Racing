using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour {

    public Button singlePlayer;
    public Button multiplayer;
    public Button controls;
    public Button options;
    public Button exitGame;

	// Use this for initialization
    void Start() {
        singlePlayer.onClick.AddListener(() => { SinglePlayer(); });
        multiplayer.onClick.AddListener(() => { Multiplayer(); });
        controls.onClick.AddListener(() => { Controls(); });
        options.onClick.AddListener(() => { Options(); });
        exitGame.onClick.AddListener(() => { ExitGame(); });
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SinglePlayer() {
        Debug.Log("SINGLE PLAYER");
        Application.LoadLevel("GameLobby");
    }

    void Multiplayer() {
        Debug.Log("MULTIPLAYER");
        Application.LoadLevel("ServerList");
    }

    void Controls() {
        Debug.Log("CONTROLS");
    }

    void Options() {
        Debug.Log("OPTIONS");
    }

    void ExitGame() {
        Debug.Log("EXIT GAME");
        Application.Quit();
    }
}
