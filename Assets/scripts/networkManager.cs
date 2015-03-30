using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class networkManager : MonoBehaviour {

    private int btnW = 160;
    private int btnH = 30;
    private int btnPadding = 10;

    // Server information
    private const string typeName = "JakesRacingGameThing";
    private string gameName = "";
    private string gamePass = "";

    private HostData[] hostList;

    // Player prefab
    public GameObject playerRacer;

    // Level loading information
    private string[] supportedNetworkLevels = new[] { "NetworkTest" };
    private string disconnectedLevel = "MainMenu";
    private int lastLevelPrefix = 0;
    private new NetworkView networkView;

    private bool loadingLevel;

    void Awake() {
    }
	// Use this for initialization
	void Start () {
        // Network level loading is done in a separate channel
        DontDestroyOnLoad(this);
        Debug.Log(gameObject.name);
        networkView = GetComponent<NetworkView>();
        networkView.group = 1;
        //Application.LoadLevel(disconnectedLevel);
	}

    // Server initialization
    public void StartServer() {
        RefreshHostList();

        Network.InitializeServer(4, 25000, !Network.HavePublicAddress());

        MasterServer.RegisterHost(typeName, gameName);
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

        SpawnPlayer();
        yield return 1;
    }

    private void SpawnPlayer() {
        float edge = 10f;
        Network.Instantiate(playerRacer, new Vector3(Random.Range(-edge, edge), .6f, Random.Range(-edge, edge)), Quaternion.identity, 0);
    }
}
