using UnityEngine;
using System.Collections;
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
    private int btnPadding = 10;

    public networkManager networkManager;
    private HostData connectingToHost;
    private string serverPass = "";
    public string connectionError;

    void Start() {
        currentMenu = MenuIndex.MainMenu;
    }

    void OnGUI() {
        switch (currentMenu) {
            case MenuIndex.MainMenu:
                firstMenu();
                break;

            case MenuIndex.ServerList:
                serverList();
                break;

            case MenuIndex.HostGame:
                hostGame();
                break;

            case MenuIndex.JoinGame:
                joinGame();
                break;
            case MenuIndex.Connecting:
                connecting();
                break;

            case MenuIndex.ConnectFail:
                connectFail();
                break;

            case MenuIndex.GameLobby:
                gameLobby();
                break;

            case MenuIndex.None:
                break;
        }
    }

    // MAIN MENU
    void firstMenu() {
        int btnW = 160;
        int btnH = 30;

        int btnX = (Screen.width - btnW) / 2;

        for (int i = 0; i < mainMenuButtons.Length; i++) {
            int btnY = (Screen.height / 2) - (btnH * (mainMenuButtons.Length - i)) + (btnPadding * i);

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
    }

    // MAIN MENU/SERVER LIST BUTTONS
    void backButtons(bool showServerList) {
        int btnW = 160;
        int btnH = 30;

        if (GUI.Button(new Rect(30, 30, btnW, btnH), "Main Menu")) {
            networkManager.LeaveServer();
            ShowMenu(MenuIndex.MainMenu);
        }

        if (showServerList) {
            if (GUI.Button(new Rect(30, 30 + btnH + btnPadding, btnW, btnH), "Server List")) {
                networkManager.LeaveServer();
                ShowMenu(MenuIndex.ServerList);
            }
        }
    }

    // SERVER LIST
    void serverList() {
        int btnW = 160;
        int btnH = 30;

        int btnX = 200;
        int btnY = 200;

        backButtons(false);

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
    }

    // HOST GAME
    void hostGame() {
        int btnW = 160;
        int btnH = 30;

        int btnX = 200;
        int btnY = 200;

        TextAnchor labelAnchor = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleRight;
        GUI.Label(new Rect(btnX, btnY + (btnH + btnPadding) * 1, btnW, btnH), "Server Name: ");
        GUI.Label(new Rect(btnX, btnY + (btnH + btnPadding) * 2, btnW, btnH), "Server Pass: ");
        GUI.skin.label.alignment = labelAnchor;

        networkManager.SetGameName(GUI.TextField(new Rect(btnX + btnW + btnPadding, btnY + (btnH + btnPadding) * 1, btnW, btnH), networkManager.GetGameName(), 25));
        networkManager.SetGamePass(GUI.TextField(new Rect(btnX + btnW + btnPadding, btnY + (btnH + btnPadding) * 2, btnW, btnH), networkManager.GetGamePass(), 25));

        backButtons(true);

        if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Start Server")) {
            if (networkManager.GetGameName() != "") {
                networkManager.StartServer();
                ShowMenu(MenuIndex.GameLobby);
            }
        }

    }

    // JOIN GAME
    void joinGame() {
        int btnW = 160;
        int btnH = 30;

        int btnX = 200;
        int btnY = 200;

        TextAnchor label2Anchor = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleRight;
        GUI.Label(new Rect(btnX, btnY + (btnH + btnPadding) * 1, btnW, btnH), "Server Pass: ");
        GUI.skin.label.alignment = label2Anchor;

        serverPass = GUI.TextField(new Rect(btnX + btnW + btnPadding, btnY + (btnH + btnPadding) * 1, btnW, btnH), serverPass, 25);

        backButtons(true);

        if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Join Server")) {
            networkManager.JoinServer(connectingToHost, serverPass);
            ShowMenu(MenuIndex.Connecting);
        }
    }

    // CONNECTING
    void connecting() {
        int lblW = 400;
        int lblH = 300;

        int lblX = (Screen.width - lblW) / 2;
        int lblY = (Screen.height - lblH) / 2;

        GUI.Label(new Rect(lblX, lblY, lblW, lblH), "CONNECTING");
    }

    // CONNECT FAIL
    void connectFail() {
        int lblW = 400;
        int lblH = 300;

        int lblX = (Screen.width - lblW) / 2;
        int lblY = (Screen.height - lblH) / 2;

        GUI.Label(new Rect(lblX, lblY, lblW, lblH), "CONNECTING");

        backButtons(true);
    }

    // GAME LOBBY
    void gameLobby() {
        int btnW = 160;
        int btnH = 30;

        int btnX = 200;
        int btnY = 200;
        backButtons(true);

        if (Network.peerType != NetworkPeerType.Disconnected) {
            if (Network.isServer) {
                string[] levels = networkManager.GetSupportedNetworkLevels();

                for (int i = 0; i < levels.Length; i++) {
                    if (GUI.Button(new Rect(btnX, btnY + (btnH + btnPadding) * i, btnW, btnH), "Add: " + levels[i])) {
                        if (networkManager.queuedLevels.Count < networkManager.maxQueue) {
                            networkManager.queuedLevels.Add(levels[i]);
                        }
                    }
                }

                if (GUI.Button(new Rect(btnX * 2, btnY * 2, btnW, btnH), "Start Game")) {
                    if (networkManager.NextLevel())
                    {
                        ShowMenu(MenuIndex.None);
                    }
                }

                for (int i = 0; i < networkManager.queuedLevels.Count; i++) {
                    if (GUI.Button(new Rect(btnX + (btnW + btnPadding), btnY + (btnH + btnPadding) * i, btnW, btnH), "Remove: " + networkManager.queuedLevels[i])) {
                        networkManager.queuedLevels.RemoveAt(i);
                    }
                }
            }
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
