using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour {

    enum MenuIndex {
        MainMenu,
        ServerList,
        GameLobby
    };

    public GameObject[] menus;

    public Button singlePlayer;
    public Button multiplayer;
    public Button controls;
    public Button options;
    public Button exitGame;

    public Button host;
    public Button join;
    public Button refresh;
    public Button menu;

    public Button addOne;
    public Button addAll;
    public Button removeOne;
    public Button removeAll;
    public Button start;
    public Button leave;

	// Use this for initialization
    void Start() {
        singlePlayer.onClick.AddListener(() => { SinglePlayer(); });
        multiplayer.onClick.AddListener(() => { Multiplayer(); });
        controls.onClick.AddListener(() => { Controls(); });
        options.onClick.AddListener(() => { Options(); });
        exitGame.onClick.AddListener(() => { ExitGame(); });

        host.onClick.AddListener(() => { Host(); });
        join.onClick.AddListener(() => { Join(); });
        refresh.onClick.AddListener(() => { Refresh(); });
        menu.onClick.AddListener(() => { Menu(); });

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

    void SinglePlayer() {
        Debug.Log("SINGLE PLAYER");
        //Application.LoadLevel("GameLobby");
        ShowMenu(MenuIndex.GameLobby);
    }

    void Multiplayer() {
        Debug.Log("MULTIPLAYER");
        //Application.LoadLevel("ServerList");
        ShowMenu(MenuIndex.ServerList);
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

    void Host() {
        Debug.Log("HOST GAME");
    }

    void Join() {
        Debug.Log("JOIN GAME");
        //Application.LoadLevel("GameLobby");
        ShowMenu(MenuIndex.GameLobby);
    }

    void Refresh() {
        Debug.Log("REFRESH LIST");
    }

    void Menu() {
        Debug.Log("MAIN MENU");
        //Application.LoadLevel("MainMenu");
        ShowMenu(MenuIndex.MainMenu);
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
        //Application.LoadLevel("MainMenu");
        ShowMenu(MenuIndex.MainMenu);
    }

    void ShowMenu(MenuIndex index) {
        for (int i = 0; i < menus.Length; i++) {
            if ((MenuIndex) i != index) {
                menus[i].SetActive(false);
            } else {
                menus[i].SetActive(true);
            }
        }
    }
}
