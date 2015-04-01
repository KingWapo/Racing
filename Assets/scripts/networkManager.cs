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

    // Level loading information
    private string[] supportedNetworkLevels = new[] { "NetworkTest" };
    private string disconnectedLevel = "MainMenu";
    private int lastLevelPrefix = 0;
    private new NetworkView networkView;

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
        networkView.group = 1;

        maxNumPlayers = 8;
        numPlayers = maxNumPlayers;

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

    void OnConnectedToServer() {
        Debug.Log("Server Joined");
        //SpawnPlayer();
    }

    void OnPlayerConnected(NetworkPlayer player) {
        playerList.Add(player);
    }

    void OnPlayerDisconnected(NetworkPlayer player) {
        Network.DestroyPlayerObjects(player);
        playerList.Remove(player);
    }

    // Disconnect from server
    void OnDisconnectedFromServer() {
        Application.LoadLevel(disconnectedLevel);
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

    public void SpawnClientRacers() {
        networkView.RPC("SpawnRacer", RPCMode.AllBuffered);
    }

    [RPC]
    public IEnumerator SpawnRacer() {
        while (loadingLevel) {
            yield return new WaitForSeconds(.1f);
        }

        for (int i = 0; i < playerList.Count; i++) {
            networkView.RPC("SpawnPlayer", RPCMode.AllBuffered, playerList[i], i);
        }


        for (int i = playerList.Count; i < maxNumPlayers; i++) {
            networkView.RPC("SpawnAI", RPCMode.AllBuffered, i);
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
            Network.Instantiate(playerRacer, start.position, start.rotation, 0);
        }
    }

    [RPC]
    private void SpawnAI(int index) {
        GameObject startSpots = GameObject.Find("Start Spots");
        startSpots spotList = startSpots.GetComponent<startSpots>();

        Debug.Log("spawned ai: " + index);
        Transform start = spotList.startPositions[index].transform;
        // CHANGE TO AI RACER PREFAB
        Network.Instantiate(playerRacer, start.position, start.rotation, 0);
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
