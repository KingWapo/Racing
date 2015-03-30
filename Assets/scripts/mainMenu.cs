using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour {

    // Menu screens
    public enum MenuIndex {
        MainMenu,
        ServerList,
        HostGame,
        GameLobby,
        None
    };

    public MenuIndex currentMenu;

    private string[] mainMenuButtons = { "Single Player", "Multiplayer", "Controls", "Options", "Exit Game" };
    private int btnX, btnY;
    private int btnW = 160;
    private int btnH = 30;
    private int btnPadding = 10;

    public networkManager networkManager;

    void Start() {
        currentMenu = MenuIndex.MainMenu;
    }

    void OnGUI() {
        switch (currentMenu) {
            case MenuIndex.MainMenu:
                btnX = (Screen.width - btnW) / 2;

                for (int i = 0; i < mainMenuButtons.Length; i++) {
                    btnY = (Screen.height / 2) - (btnH * (mainMenuButtons.Length - i)) + (btnPadding * i);

                    if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), mainMenuButtons[i])) {
                        switch (i) {
                            case 0:
                                Debug.Log("SinglePlayer");
                                networkManager.SetGameName("private game");
                                networkManager.StartPrivateServer();
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
                    networkManager.RefreshHostList();
                }

                HostData[] hostList = networkManager.GetHostList();

                if (hostList != null) {
                    for (int i = 0; i < hostList.Length; i++) {
                        if (GUI.Button(new Rect(btnX, btnY + (btnH + btnPadding) * (i + 1), btnW * 3, btnH), hostList[i].gameName)) {
                            networkManager.JoinServer(hostList[i]);
                            ShowMenu(MenuIndex.GameLobby);
                        }
                    }
                }

                break;
            case MenuIndex.HostGame:
                btnX = 200;
                btnY = 200;

                networkManager.SetGameName(GUI.TextField(new Rect(btnX, btnY + (btnH + btnPadding) * 1, btnW, btnH), networkManager.GetGameName(), 25));
                networkManager.SetGamePass(GUI.TextField(new Rect(btnX, btnY + (btnH + btnPadding) * 2, btnW, btnH), networkManager.GetGamePass(), 25));

                if (GUI.Button(new Rect(30, 30, btnW, btnH), "Main Menu")) {
                    ShowMenu(MenuIndex.MainMenu);
                }

                if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Start Server")) {
                    if (networkManager.GetGameName() != "") {
                        networkManager.StartServer();
                        ShowMenu(MenuIndex.GameLobby);
                    }
                }

                break;
            case MenuIndex.GameLobby:
                if (Network.peerType != NetworkPeerType.Disconnected) {
                    string[] levels = networkManager.GetSupportedNetworkLevels();

                    for (int i = 0; i < levels.Length; i++) {
                        if (GUI.Button(new Rect(100, 50 + (60 * i), 160, 50), levels[i])) {
                            Network.RemoveRPCsInGroup(0);
                            Network.RemoveRPCsInGroup(1);
                            networkManager.LoadNewLevel(levels[i]);
                            networkManager.SpawnClientRacers();
                            ShowMenu(MenuIndex.None);
                        }
                    }
                }

                break;
            case MenuIndex.None:
                if (GUI.Button(new Rect(30, 30, btnH, btnH), "X")) {
                    Network.DestroyPlayerObjects(Network.player);
                    Network.Disconnect();
                    Application.Quit();
                }

                break;
        }
    }

    // Displayed menu
    void ShowMenu(MenuIndex index) {
        currentMenu = index;

        if (index == MenuIndex.ServerList) {
            networkManager.RefreshHostList();
        }
    }
}
