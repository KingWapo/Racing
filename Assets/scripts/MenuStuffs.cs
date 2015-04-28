using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum Menu {
    main, servers, lobby, controls, credits, host, join, MAX
};

public class MenuStuffs : MonoBehaviour {

    public networkManager netManager;

    public GameObject main;
    public GameObject servers;
    public GameObject lobby;
    public GameObject controls;
    public GameObject credits;
    public GameObject host;
    public GameObject join;

    public GameObject hostServerName;
    public GameObject hostServerPass;
    public GameObject hostDisplayName;
    public GameObject hostErrorText;

    public GameObject joinServerPass;
    public GameObject joinDisplayName;
    public GameObject joinErrorText;

    public GameObject lobbyPlayerList;

    private Menu currMenu;

    public GameObject serverListDisplay;
    public GameObject serverButton;

    public GameObject lobbyQueuedMaps;
    public GameObject lobbyAvailableMaps;
    public GameObject mapButton;

    private HostData requestedHost;

    void Start() {
        currMenu = Menu.MAX;

        DisplayMenu(Menu.main);
    }

    public void GotoMain() {
        requestedHost = null;
        DisplayMenu(Menu.main);

        netManager.LeaveServer();
    }

    public void GotoServers() {
        DisplayMenu(Menu.servers);

        netManager.LeaveServer();
    }

    public void RefreshServers() {
        netManager.RefreshHostList();

        Transform serverList = serverListDisplay.transform;
        foreach (Transform child in serverList) {
            Destroy(child.gameObject);
        }

        HostData[] hostList = netManager.GetHostList();

        // DONT DELETE THIS BECAUSE UNITY IS STUPID
        Debug.Log("REFRESHING: " + hostList.Length);

        if (hostList != null) {

            for (int i = 0; i < hostList.Length; i++) {
               if (hostList[i].connectedPlayers < hostList[i].playerLimit) {
                    GameObject newButton = (GameObject)Instantiate(serverButton);
                    Text[] texts = newButton.GetComponentsInChildren<Text>();

                    Debug.Log("texts length: " + texts.Length);
                    texts[0].text = hostList[i].gameName;
                    texts[1].text = "Players\n" + hostList[i].connectedPlayers + "/" + hostList[i].playerLimit;
                    texts[2].text = "Password: ";

                    if (hostList[i].passwordProtected) {
                        texts[2].text += "yes";
                    } else {
                        texts[2].text += "no";
                    }

                    newButton.transform.SetParent(serverListDisplay.transform);
                    Button b = newButton.GetComponent<Button>();

                    HostData hostData = hostList[i];
                    b.onClick.AddListener(() => JoinGame(hostData));
                }
            }
        } else {
            Debug.Log("no host list");
        }
    }

    public void GotoLobby() {
        DisplayMenu(Menu.lobby);

        netManager.UpdateClientLevels();
    }

    public void UpdateLevelsList(string[] queuedLevels) {
        string[] levels = netManager.GetSupportedNetworkLevels();

        Transform availableTransform = lobbyAvailableMaps.transform;
        foreach (Transform child in availableTransform) {
            Destroy(child.gameObject);
        }

        Transform queuedTransform = lobbyQueuedMaps.transform;
        foreach (Transform child in queuedTransform) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < levels.Length; i++) {
            GameObject newButton = (GameObject)Instantiate(mapButton);
            Text text = newButton.GetComponentInChildren<Text>();

            newButton.transform.SetParent(lobbyAvailableMaps.transform);
            Button b = newButton.GetComponent<Button>();

            text.text = levels[i];

            if (!Network.isServer) {
                b.interactable = false;
            }

            string level = levels[i];

            b.onClick.AddListener(() => AddLevel(level));
        }

        for (int i = 0; i < queuedLevels.Length; i++) {
            GameObject newButton = (GameObject)Instantiate(mapButton);
            Text text = newButton.GetComponentInChildren<Text>();

            newButton.transform.SetParent(lobbyQueuedMaps.transform);
            Button b = newButton.GetComponent<Button>();

            text.text = queuedLevels[i];

            if (!Network.isServer) {
                b.interactable = false;
            }

            int index = i;

            b.onClick.AddListener(() => RemoveLevel(index));
        }
    }

    public void UpdateLobbyPlayerList(string[] playerList) {
        MenuStuffs menu = FindObjectOfType<MenuStuffs>();
        Text list = menu.lobbyPlayerList.GetComponent<Text>();
        list.text = "";

        for (int i = 0; i < playerList.Length; i++) {
            list.text += playerList[i] + "\n";
        }
    }

    private void AddLevel(string level) {
        netManager.queuedLevels.Add(level);

        netManager.UpdateClientLevels();
    }

    private void RemoveLevel(int i) {
        netManager.queuedLevels.RemoveAt(i);

        netManager.UpdateClientLevels();
    }

    public void GotoControls() {
        DisplayMenu(Menu.controls);
    }

    public void GotoCredits() {
        DisplayMenu(Menu.credits);
    }

    public void HostGame() {
        DisplayMenu(Menu.host);
    }

    public void JoinGame(HostData hostData) {
        requestedHost = hostData;
        DisplayMenu(Menu.join);

        if (hostData.passwordProtected) {
            joinServerPass.GetComponent<InputField>().interactable = true;
        } else {
            joinServerPass.GetComponent<InputField>().interactable = false;
        }
    }

    public void JoinGame() {
        string sPass = joinServerPass.GetComponentsInChildren<Text>()[1].text;
        string dName = joinDisplayName.GetComponentsInChildren<Text>()[1].text;

        if (dName.Trim() == "") {
            joinErrorText.SetActive(true);
            joinErrorText.GetComponent<Text>().text = "DISPLAY NAME REQUIRED";
        } else if (requestedHost != null) {
            if (!requestedHost.passwordProtected) {
                netManager.JoinServer(requestedHost, dName);
            } else {
                netManager.JoinServer(requestedHost, sPass, dName);
            }
        } else {
            Debug.Log("Something went wrong");
        }
    }

    public void StartServer() {
        string sName = hostServerName.GetComponentsInChildren<Text>()[1].text;
        string sPass = hostServerPass.GetComponentsInChildren<Text>()[1].text;
        string dName = hostDisplayName.GetComponentsInChildren<Text>()[1].text;

        if (sName.Trim() == "") {
            hostErrorText.SetActive(true);
            hostErrorText.GetComponentInChildren<Text>().text = "SERVER NAME REQUIRED";
        } else if (dName.Trim() == "") {
            hostErrorText.SetActive(true);
            hostErrorText.GetComponentInChildren<Text>().text = "DISPLAY NAME REQUIRED";
        } else {
            hostErrorText.SetActive(false);
            netManager.SetGameName(sName);
            netManager.SetGamePass(sPass);
            netManager.StartServer(dName);

            GotoLobby();
        }
    }

    public void StartPrivateServer() {
        netManager.StartPrivateServer();
        GotoLobby();
    }

    public void StartGame() {
        if (Network.isServer) {
            if (netManager.queuedLevels.Count > 0) {
                netManager.NextLevel();
            }
        }
    }

    public void DisplayMenu(Menu destination) {
        if (currMenu != destination) {

            main.SetActive(false);
            servers.SetActive(false);
            lobby.SetActive(false);
            controls.SetActive(false);
            credits.SetActive(false);
            host.SetActive(false);
            join.SetActive(false);

            switch (destination) {
                case Menu.main:
                    main.SetActive(true);
                    break;
                case Menu.servers:
                    servers.SetActive(true);
                    RefreshServers();
                    break;
                case Menu.lobby:
                    lobby.SetActive(true);
                    break;
                case Menu.controls:
                    controls.SetActive(true);
                    break;
                case Menu.credits:
                    credits.SetActive(true);
                    break;
                case Menu.host:
                    host.SetActive(true);
                    break;
                case Menu.join:
                    join.SetActive(true);
                    break;
            }

            currMenu = destination;
        }
    }
}
