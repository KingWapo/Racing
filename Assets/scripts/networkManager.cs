using UnityEngine;
using System.Collections;

public class networkManager : MonoBehaviour {

    // Menu screens
    enum MenuIndex {
        MainMenu,
        ServerList,
        HostGame,
        GameLobby,
        None
    };

    private MenuIndex currentMenu;

    private int btnW = 160;
    private int btnH = 30;
    private int btnPadding = 10;

    // Main menu button text
    string[] mainMenuButtons = { "Single Player", "Multiplayer", "Controls", "Options", "Exit Game" };

    // Server information
    private const string typeName = "JakesRacingGameThing";
    private string gameName = "";
    private string gamePass = "";

    private HostData[] hostList;

    // Player prefab
    public GameObject playerRacer;

	// Use this for initialization
	void Start () {
        currentMenu = MenuIndex.MainMenu;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        int btnX = 0;
        int btnY = 0;

        switch (currentMenu) {
            case MenuIndex.MainMenu:
                btnX = (Screen.width - btnW) / 2;
                for (int i = 0; i < mainMenuButtons.Length; i++) {
                    btnY = (Screen.height / 2) - (btnH * (mainMenuButtons.Length - i)) + (btnPadding * i);

                    if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), mainMenuButtons[i])) {
                        switch (i) {
                            case 0:
                                Debug.Log("SinglePlayer");
                                ShowMenu(MenuIndex.GameLobby);
                                break;
                            case 1:
                                Debug.Log("Multiplayer");
                                ShowMenu(MenuIndex.ServerList);
                                break;
                            case 2:
                                Debug.Log("Controls");
                                break;
                            case 3:
                                Debug.Log("Options");
                                break;
                            case 4:
                                Debug.Log("ExitGame");
                                Application.Quit();
                                break;
                            default:
                                break;
                        }
                    }
                }
                
                break;
            case MenuIndex.ServerList:
                btnX = 200;
                btnY = 200;

                if (GUI.Button(new Rect(30, 30, btnW, btnH), "Main Menu")) {
                    ShowMenu(MenuIndex.MainMenu);
                }

                if (!Network.isClient && !Network.isServer) {
                    if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Host Game")) {
                        ShowMenu(MenuIndex.HostGame);
                    }
                }

                if (GUI.Button(new Rect(btnX + btnW + btnPadding, btnY, btnW, btnH), "Refresh List")) {
                    RefreshHostList();
                }

                if (hostList != null) {
                    for (int i = 0; i < hostList.Length; i++) {
                        if (GUI.Button(new Rect(btnX, btnY + (btnH + btnPadding) * (i + 1), btnW * 3, btnH), hostList[i].gameName)) {
                            JoinServer(hostList[i]);
                            ShowMenu(MenuIndex.None);
                        }
                    }
                }

                break;
            case MenuIndex.HostGame:
                btnX = 200;
                btnY = 200;

                gameName = GUI.TextField(new Rect(btnX, btnY + (btnH + btnPadding) * 1, btnW, btnH), gameName, 25);
                gamePass = GUI.TextField(new Rect(btnX, btnY + (btnH + btnPadding) * 2, btnW, btnH), gamePass, 25);

                if (GUI.Button(new Rect(30, 30, btnW, btnH), "Main Menu")) {
                    ShowMenu(MenuIndex.MainMenu);
                }

                if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Start Server")) {
                    if (gameName != "") {
                        StartServer();
                        ShowMenu(MenuIndex.None);
                    }
                }

                break;
            case MenuIndex.GameLobby:
                break;
            case MenuIndex.None:
                if (GUI.Button(new Rect(30, 30, btnH, btnH), "X")) {
                    Application.Quit();
                }
                break;
        }
    }

    // Server initialization
    private void StartServer() {
        RefreshHostList();

        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());

        MasterServer.RegisterHost(typeName, gameName);
    }

    void OnServerInitialized() {
        Debug.Log("Server Initialized");
        SpawnPlayer();
    }

    // Refresh host list
    private void RefreshHostList() {
        MasterServer.RequestHostList(typeName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent) {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostList = MasterServer.PollHostList();
    }

    // Join existing server
    private void JoinServer(HostData hostData) {
        Network.Connect(hostData);
    }

    void OnConnectedToServer() {
        Debug.Log("Server Joined");
        SpawnPlayer();
    }

    // Displayed menu
    void ShowMenu(MenuIndex index) {
        currentMenu = index;

        if (index == MenuIndex.ServerList) {
            RefreshHostList();
        }
    }

    private void SpawnPlayer() {
        float edge = 3f;
        Network.Instantiate(playerRacer, new Vector3(Random.Range(-edge, edge), .6f, Random.Range(-edge, edge)), Quaternion.identity, 0);
    }
}
