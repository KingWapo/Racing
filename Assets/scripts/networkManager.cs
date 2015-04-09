using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    void Awake() {
    }
	// Use this for initialization
	void Start () {
        // Network level loading is done in a separate channel
        DontDestroyOnLoad(this);
        Debug.Log(gameObject.name);
        networkView = GetComponent<NetworkView>();
        racingManager = GetComponent<RacingManager>();
        networkView.group = 1;

        maxNumPlayers = 7; // number of players other than the server
        numPlayers = maxNumPlayers;

        queuedLevels = new List<string>();
        playerList = new List<NetworkPlayer>();
	}

    // Server initialization
    public void StartServer() {
        RefreshHostList();

        Network.incomingPassword = gamePass;
        Network.InitializeServer(numPlayers, 25000, !Network.HavePublicAddress());

        MasterServer.RegisterHost(typeName, gameName);

        playerList.Add(Network.player);
    }

    public void StartPrivateServer() {
        gamePass = "myPrivateServer";
        numPlayers = 1;
        StartServer();
    }

    void OnServerInitialized() {
        Debug.Log("Server Initialized");
        //SpawnPlayer();
    }

    // Refresh host list
    public void RefreshHostList() {
        MasterServer.RequestHostList(typeName);
    }

    void OnMasterServerEvent(MasterServerEvent msEvent) {
        if (msEvent == MasterServerEvent.HostListReceived)
            hostList = MasterServer.PollHostList();
    }

    // Join existing server
    public void JoinServer(HostData hostData) {
        Network.Connect(hostData);
    }

    public void JoinServer(HostData hostData, string pass) {
        Network.Connect(hostData, pass);
    }

    void OnConnectedToServer() {
        Debug.Log("Server Joined");

        mainMenu menu = FindObjectOfType<mainMenu>();

        menu.ShowMenu(MenuIndex.GameLobby);
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
        playerList.Remove(player);
    }

    void OnFailedToConnect(NetworkConnectionError error) {
        mainMenu menu = FindObjectOfType<mainMenu>();

        menu.connectionError = "Failed to connect to server: " + error;
        menu.ShowMenu(MenuIndex.ConnectFail);
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

    public bool NextLevel()
    {
        if (queuedLevels.Count > 0)
        {
            Network.RemoveRPCsInGroup(0);
            Network.RemoveRPCsInGroup(1);
            LoadNewLevel(queuedLevels[0]);
            queuedLevels.RemoveAt(0);
            SpawnClientRacers();
            return true;
        }
        else
        {

        }
        return false;
    }

    public void LoadNewLevel(string level) {
        networkView.RPC("LoadLevel", RPCMode.AllBuffered, level, lastLevelPrefix + 1);
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

        networkView.RPC("SpawnPlayerShooter", RPCMode.AllBuffered, playerList[0], 0);

        // TODO change back to 0 when done testing shooter
        for (int i = 1; i < playerList.Count; i++) {
            networkView.RPC("SpawnPlayer", RPCMode.AllBuffered, playerList[i], i);
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
        if (Network.isServer) {
            int arrayindex = -1;
            for (int i = 0; i < playerList.Count; i++) {
                GUI.Label(new Rect(20, 200 + 80 * i, 160, 80), "Index: " + i + "\nPlayerID: " + playerList[i]);

                if (playerList[i] == Network.player)
                    arrayindex = i;
            }

            GUI.Label(new Rect(20, 20, 160, 80), "Connections: " + playerList.Count + "\nNetwork ID: " + Network.player.ToString() + "\nArray Index: " + arrayindex);
        }
    }
}
