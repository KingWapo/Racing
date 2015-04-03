using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// Menu screens
public enum MenuIndex {
    MainMenu,
    ServerList,
    HostGame,
    JoinGame,
    Connecting,
    ConnectFail,
    GameLobby,
    None
};

public class mainMenu : MonoBehaviour {

    public MenuIndex currentMenu;

    private string[] mainMenuButtons = { "Single Player", "Multiplayer", "Controls", "Options", "Exit Game" };
    private int btnX, btnY;
    private int btnW = 160;
    private int btnH = 30;
    private int btn2W = 400;
    private int btn2H = 300;
    private int btnPadding = 10;

    public networkManager networkManager;
    private HostData connectingToHost;
    private string serverPass = "";
    public string connectionError;

    private List<string> queuedLevels;

    void Start() {
        currentMenu = MenuIndex.MainMenu;
        queuedLevels = new List<string>();
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
                    TextAnchor buttonAnchor = GUI.skin.button.alignment;
                    GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                    for (int i = 0; i < hostList.Length; i++) {
                        string serverInfo = hostList[i].gameName + "\t" + hostList[i].connectedPlayers + "/" + hostList[i].playerLimit;
                        if (hostList[i].connectedPlayers < hostList[i].playerLimit) {
                            if (GUI.Button(new Rect(btnX, btnY + (btnH + btnPadding) * (i + 1), btnW * 3, btnH), serverInfo)) {
                                if (hostList[i].passwordProtected) {
                                    connectingToHost = hostList[i];
                                    ShowMenu(MenuIndex.JoinGame);
                                } else {
                                    networkManager.JoinServer(hostList[i]);
                                    ShowMenu(MenuIndex.GameLobby);
                                }
                            }
                        }
                    }
                    GUI.skin.button.alignment = buttonAnchor;
                }

                break;
            case MenuIndex.HostGame:
                btnX = 200;
                btnY = 200;

                TextAnchor labelAnchor = GUI.skin.label.alignment;
                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                GUI.Label(new Rect(btnX, btnY + (btnH + btnPadding) * 1, btnW, btnH), "Server Name: ");
                GUI.Label(new Rect(btnX, btnY + (btnH + btnPadding) * 2, btnW, btnH), "Server Pass: ");
                GUI.skin.label.alignment = labelAnchor;

                networkManager.SetGameName(GUI.TextField(new Rect(btnX + btnW + btnPadding, btnY + (btnH + btnPadding) * 1, btnW, btnH), networkManager.GetGameName(), 25));
                networkManager.SetGamePass(GUI.TextField(new Rect(btnX + btnW + btnPadding, btnY + (btnH + btnPadding) * 2, btnW, btnH), networkManager.GetGamePass(), 25));

                if (GUI.Button(new Rect(30, 30, btnW, btnH), "Main Menu")) {
                    ShowMenu(MenuIndex.MainMenu);
                }

                if (GUI.Button(new Rect(30, 30 + btnH + btnPadding, btnW, btnH), "Server List")) {
                    ShowMenu(MenuIndex.ServerList);
                }

                if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Start Server")) {
                    if (networkManager.GetGameName() != "") {
                        networkManager.StartServer();
                        ShowMenu(MenuIndex.GameLobby);
                    }
                }

                break;
            case MenuIndex.JoinGame:
                btnX = 200;
                btnY = 200;

                TextAnchor label2Anchor = GUI.skin.label.alignment;
                GUI.skin.label.alignment = TextAnchor.MiddleRight;
                GUI.Label(new Rect(btnX, btnY + (btnH + btnPadding) * 1, btnW, btnH), "Server Pass: ");
                GUI.skin.label.alignment = label2Anchor;

                serverPass = GUI.TextField(new Rect(btnX + btnW + btnPadding, btnY + (btnH + btnPadding) * 1, btnW, btnH), serverPass, 25);

                if (GUI.Button(new Rect(30, 30, btnW, btnH), "Main Menu")) {
                    ShowMenu(MenuIndex.MainMenu);
                }

                if (GUI.Button(new Rect(30, 30 + btnH + btnPadding, btnW, btnH), "Server List")) {
                    ShowMenu(MenuIndex.ServerList);
                }

                if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Join Server")) {
                    networkManager.JoinServer(connectingToHost, serverPass);
                    ShowMenu(MenuIndex.Connecting);
                }

                break;
            case MenuIndex.Connecting:
                GUI.Label(new Rect((Screen.width - btn2W) / 2, (Screen.height - btn2H) / 2, btn2W, btn2H), "CONNECTING");
                break;
            case MenuIndex.ConnectFail:
                GUI.Label(new Rect((Screen.width - btn2W) / 2, (Screen.height - btn2H) / 2, btn2W, btn2H), connectionError);

                if (GUI.Button(new Rect(30, 30, btnW, btnH), "Server List")) {
                    ShowMenu(MenuIndex.ServerList);
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
    public void ShowMenu(MenuIndex index) {
        currentMenu = index;

        if (index == MenuIndex.ServerList) {
            networkManager.RefreshHostList();
        }
    }
}
