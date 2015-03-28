using UnityEngine;
using System.Collections;

public class networkManager : MonoBehaviour {

    // Menu screens
    enum MenuIndex {
        MainMenu,
        ServerList,
        GameLobby
    };

    private MenuIndex currentMenu;

    private int btnW = 160;
    private int btnH = 30;
    private int btnPadding = 10;

    // Main menu button text
    string[] mainMenuButtons = { "Single Player", "Multiplayer", "Controls", "Options", "Exit Game" };

    // Server information
    private const string typeName = "JakesRacingGameThing";
    private const string gameName = "BrandonLaptop";

    private HostData[] hostList;

	// Use this for initialization
	void Start () {
        currentMenu = MenuIndex.MainMenu;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI() {
        switch (currentMenu) {
            case MenuIndex.MainMenu:
                for (int i = 0; i < mainMenuButtons.Length; i++) {
                    if (GUI.Button(new Rect((Screen.width - btnW) / 2, (Screen.height / 2) - (btnH * (mainMenuButtons.Length - i)) + (btnPadding * i), btnW, btnH), mainMenuButtons[i])) {
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
                int btnX = 200;
                int btnY = 200;
                if (GUI.Button(new Rect(30, 30, btnW, btnH), "Main Menu")) {
                    ShowMenu(MenuIndex.MainMenu);
                }

                if (!Network.isClient && !Network.isServer) {
                    if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Start Server")) {
                        StartServer();
                    }
                }

                if (GUI.Button(new Rect(btnX + btnW + btnPadding, btnY, btnW, btnH), "Refresh List")) {
                    RefreshHostList();
                }

                if (hostList != null) {
                    for (int i = 0; i < hostList.Length; i++) {
                        if (GUI.Button(new Rect(btnX, btnY + (btnH + btnPadding) * (i + 1), btnW * 3, btnH), hostList[i].gameName)) {
                            JoinServer(hostList[i]);
                        }
                    }
                }

                break;
            case MenuIndex.GameLobby:
                break;
        }
    }

    // Server initialization
    private void StartServer() {
        RefreshHostList();

        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());

        string fixedGameName = gameName;

        // remove when finished - bfelch
        if (hostList != null) {
            fixedGameName += hostList.Length;
        } else {
            fixedGameName += "0";
        }

        MasterServer.RegisterHost(typeName, fixedGameName);
    }

    void OnServerInitialized() {
        Debug.Log("Server Initialized");
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
    }

    // Displayed menu
    void ShowMenu(MenuIndex index) {
        currentMenu = index;
    }
}
