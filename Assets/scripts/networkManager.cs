using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkView))]
public class networkManager : MonoBehaviour {

    // Server information
    private const string typeName = "JakesRacingGameThing";
    private string gameName = "";
    private string gamePass = "";
    private int maxNumPlayers;
    private int numPlayers;

    private HostData[] hostList;

    // Player prefab
    public GameObject playerRacer;
    public GameObject playerShooter;

    // Level loading information
    private string[] supportedNetworkLevels = new[] { "NetworkTest" };
    public List<string> queuedLevels;
    public int maxQueue = 5;
    private string disconnectedLevel = "MainMenu";
    private int lastLevelPrefix = 0;

    private new NetworkView networkView;
    private RacingManager racingManager;

    private bool loadingLevel;

    private List<NetworkPlayer> playerList;
    private List<string> nameList;

    private string connectingName;

    private bool receivedHostList;

    void Awake() {
    }
	// Use this for initialization
	void Start () {
        // Network level loading is done in a separate channel
        DontDestroyOnLoad(this);
        //Debug.Log(gameObject.name);
        networkView = GetComponent<NetworkView>();
        racingManager = GetComponent<RacingManager>();
        networkView.group = 1;

        maxNumPlayers = 7; // number of players other than the server
        numPlayers = maxNumPlayers;

        queuedLevels = new List<string>();
        playerList = new List<NetworkPlayer>();
        nameList = new List<string>();
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            NextLevel();
            LeaveServer();
        }
    }

    // Server initialization
    public void StartServer(string displayName) {
        RefreshHostList();

        Network.incomingPassword = gamePass;
        Network.InitializeServer(numPlayers, 25000, !Network.HavePublicAddress());

        MasterServer.RegisterHost(typeName, gameName);

        numPlayers = maxNumPlayers;
        playerList.Add(Network.player);
        nameList.Add(displayName);
        UpdateClientPlayers();
    }

    public void StartPrivateServer() {
        //gameName = "thisismyprivateserverleavenow";
        gamePass = "myPrivateServer";
        numPlayers = 0;
        StartServer("femur");
    }

    void OnServerInitialized() {
        Debug.Log("Server Initialized");
        //SpawnPlayer();
    }

    // Refresh host list
    public void RefreshHostList() {
        MasterServer.RequestHostList(typeName);
        receivedHostList = false;
    }

    void OnMasterServerEvent(MasterServerEvent msEvent) {
        if (msEvent == MasterServerEvent.HostListReceived) {
            hostList = MasterServer.PollHostList();
            receivedHostList = true;
        } else if (msEvent == MasterServerEvent.RegistrationSucceeded) {
            Debug.Log("Server is hosted!");
        }
    }

    public bool didReceiveHostList() {
        return receivedHostList;
    }

    // Join existing server
    public void JoinServer(HostData hostData, string displayName) {
        Network.Connect(hostData);
        connectingName = displayName;
    }

    public void JoinServer(HostData hostData, string pass, string displayName) {
        Network.Connect(hostData, pass);
        connectingName = displayName;
    }

    void OnConnectedToServer() {
        Debug.Log("Server Joined");

        MenuStuffs menu = FindObjectOfType<MenuStuffs>();

        networkView.RPC("AddPlayer", RPCMode.Server, connectingName);

        menu.DisplayMenu(Menu.lobby);
    }

    void OnPlayerConnected(NetworkPlayer player) {
        playerList.Add(player);
    }

    // Disconnect from server
    public void LeaveServer() {
        Network.Disconnect();
    }

    void OnDisconnectedFromServer() {
        Application.LoadLevel(disconnectedLevel);
    }

    void OnPlayerDisconnected(NetworkPlayer player) {
        Network.DestroyPlayerObjects(player);
        racingManager.RemoveRacer(player);

        int i = playerList.IndexOf(player);
        playerList.Remove(player);
        nameList.RemoveAt(i);

        UpdateClientPlayers();
    }

    void OnFailedToConnect(NetworkConnectionError error) {
        MenuStuffs menu = FindObjectOfType<MenuStuffs>();

        menu.DisplayMenu(Menu.join);
        menu.joinErrorText.GetComponent<Text>().text = "Failed to connect to server: " + error;
    }

    public HostData[] GetHostList() {
        return hostList;
    }

    public string GetGameName() {
        return gameName;
    }

    public void SetGameName(string name) {
        gameName = name;
    }

    public string GetGamePass() {
        return gamePass;
    }

    public void SetGamePass(string pass) {
        gamePass = pass;
    }

    public bool isLoadingLevel() {
        return loadingLevel;
    }

    public string[] GetSupportedNetworkLevels() {
        return supportedNetworkLevels;
    }

    public void EndMatch()
    {
        NextLevel();
    }

    public bool NextLevel()
    {
        if (queuedLevels.Count > 0)
        {
            Network.RemoveRPCsInGroup(0);
            Network.RemoveRPCsInGroup(1);
            LoadNewLevel(queuedLevels[0]);
            queuedLevels.RemoveAt(0);
            SpawnClients();
            return true;
        }
        else
        {
            networkView.RPC("LoadLevel", RPCMode.AllBuffered, disconnectedLevel, lastLevelPrefix + 1);

            LeaveServer();
        }
        return false;
    }

    public void LoadNewLevel(string level) {
        networkView.RPC("LoadLevel", RPCMode.AllBuffered, level, lastLevelPrefix + 1);
    }

    public void UpdateClientPlayers() {
        string players = "";

        for (int i = 0; i < playerList.Count; i++) {
            players += nameList[i];

            if (i < playerList.Count - 1) {
                players += ",";
            }
        }

        networkView.RPC("UpdatePlayerList", RPCMode.AllBuffered, players);
    }

    [RPC]
    void UpdatePlayerList(string players) {
        MenuStuffs menu = FindObjectOfType<MenuStuffs>();

        string[] playersInGame = new string[0];

        if (players != "") {
            playersInGame = players.Split(',');
        }

        nameList.Clear();

        foreach (string name in playersInGame) {
            nameList.Add(name);
        }

        Debug.Log("number of players: " + playersInGame.Length);
        menu.UpdateLobbyPlayerList(playersInGame);
    }

    public void UpdateClientLevels() {
        string levels = "";

        for (int i = 0; i < queuedLevels.Count; i++) {
            levels += queuedLevels[i];

            if (i < queuedLevels.Count - 1) {
                levels += ",";
            }
        }

        Debug.Log("levels string: " + levels);
        networkView.RPC("UpdateLevelsList", RPCMode.AllBuffered, levels);
    }

    [RPC]
    void UpdateLevelsList(string levels) {
        MenuStuffs menu = FindObjectOfType<MenuStuffs>();

        string[] queueLevels = new string[0];

        if (levels != "") {
            queueLevels = levels.Split(',');
        }

        queuedLevels.Clear();
        foreach (string level in queueLevels) {
            queuedLevels.Add(level);
        }

        Debug.Log("number of levels: " + queueLevels.Length);
        menu.UpdateLevelsList(queueLevels);
    }

    [RPC]
    void AddPlayer(string newPlayer) {
        nameList.Add(newPlayer);
        UpdateClientPlayers();
    }

    // Load Levels
    [RPC]
    IEnumerator LoadLevel(string level, int levelPrefix) {
        lastLevelPrefix = levelPrefix;
        loadingLevel = true;

        // Stop sending data so we can load level
        Network.SetSendingEnabled(0, false);

        // Stop receiving data so we can load level;
        Network.isMessageQueueRunning = false;

        // Give network view id a prefix to prevent old updates from clients
        Network.SetLevelPrefix(levelPrefix);
        Application.LoadLevel(level);
        Debug.Log("Gone to level - " + level);
        yield return 1;

        // Allow receiving data again
        Network.isMessageQueueRunning = true;

        // Allow sending data again
        Network.SetSendingEnabled(0, true);

        GameObject[] gameObjects = FindObjectsOfType<GameObject>();

        foreach(GameObject go in gameObjects) {
            go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
        }

        loadingLevel = false;
    }

    public void SpawnClients() {
        networkView.RPC("SpawnClient", RPCMode.AllBuffered);
    }

    [RPC]
    public IEnumerator SpawnClient() {
        while (loadingLevel) {
            yield return new WaitForSeconds(.1f);
        }

        int turretIndex = Random.Range(0, playerList.Count);

        if (playerList.Count >= 2) {
            networkView.RPC("SpawnPlayerShooter", RPCMode.AllBuffered, playerList[turretIndex], turretIndex);

            if (Network.isServer) {
                networkView.RPC("SpawnAI", RPCMode.AllBuffered, turretIndex);
            }
        }

        for (int i = 0; i < playerList.Count; i++) {
            if (playerList.Count < 2 || i != turretIndex) {
                networkView.RPC("SpawnPlayer", RPCMode.AllBuffered, playerList[i], i);
            }
        }

        // only let server call function
        if (Network.isServer) {
            for (int i = playerList.Count; i <= maxNumPlayers; i++) {
                networkView.RPC("SpawnAI", RPCMode.AllBuffered, i);
            }
        }

        yield return 1;
    }

    [RPC]
    private void SpawnPlayer(NetworkPlayer netPlayer, int index) {
        GameObject startSpots = GameObject.Find("Start Spots");
        startSpots spotList = startSpots.GetComponent<startSpots>();

        if (netPlayer == Network.player) {
            Debug.Log("spawned player: " + index);
            Transform start = spotList.startPositions[index].transform;
            GameObject racer = (GameObject) Network.Instantiate(playerRacer, start.position, start.rotation, 0);
            racer.AddComponent<PlayerController>();
            racer.GetComponent<playerRacer>().SetStartPoint(start.position, start.rotation);
            racer.GetComponent<playerRacer>().playerName = nameList[index];

            racingManager.AddRacer(racer, index);
        }
    }

    [RPC]
    private void SpawnAI(int index) {
        // only let server execute function
        // need both isServer checks to prevent dupe spawns
        if (Network.isServer) {
            GameObject startSpots = GameObject.Find("Start Spots");
            startSpots spotList = startSpots.GetComponent<startSpots>();

            Debug.Log("spawned ai: " + index);
            Transform start = spotList.startPositions[index].transform;
            GameObject racer = (GameObject)Network.Instantiate(playerRacer, start.position, start.rotation, 0);
            racer.AddComponent<AIController>();
            racer.GetComponent<playerRacer>().SetStartPoint(start.position, start.rotation);
            racer.GetComponent<playerRacer>().playerName = "AI Racer";

            racingManager.AddRacer(racer, index);
        }
    }

    [RPC]
    private void SpawnPlayerShooter(NetworkPlayer netPlayer, int index) {
        if (netPlayer == Network.player) {
            GameObject spawn = GameObject.Find("ShootingTrackLocation");
            Debug.Log("spawned player shooter: " + index);
            GameObject shooter = (GameObject)Network.Instantiate(playerShooter, spawn.transform.position, Quaternion.identity, 0);
            shooter.AddComponent<playerShootController>();
        }
    }

    [RPC]
    private void SpawnAIShooter(int index) {
        // only let server execute function
        // need both isServer checks to prevent dupe spawns
        if (Network.isServer) {
            Debug.Log("spawned ai shooter: " + index);
            GameObject shooter = (GameObject)Network.Instantiate(playerShooter, Vector3.zero, Quaternion.identity, 0);
            //racer.AddComponent<AIController>();
        }
    }

    void OnGUI() {
        /*if (Network.isServer) {
            int arrayindex = -1;
            for (int i = 0; i < playerList.Count; i++) {
                GUI.Label(new Rect(20, 200 + 80 * i, 160, 80), "Index: " + i + "\nPlayerID: " + playerList[i]);

                if (playerList[i] == Network.player)
                    arrayindex = i;
            }

            GUI.Label(new Rect(20, 20, 160, 80), "Connections: " + playerList.Count + "\nNetwork ID: " + Network.player.ToString() + "\nArray Index: " + arrayindex);
        }*/
    }
}
