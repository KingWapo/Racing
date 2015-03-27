using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class serverList : MonoBehaviour {

    public Button host;
    public Button join;
    public Button refresh;
    public Button menu;

	// Use this for initialization
    void Start() {
        host.onClick.AddListener(() => { Host(); });
        join.onClick.AddListener(() => { Join(); });
        refresh.onClick.AddListener(() => { Refresh(); });
        menu.onClick.AddListener(() => { Menu(); });
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Host() {
        Debug.Log("HOST GAME");
    }

    void Join() {
        Debug.Log("JOIN GAME");
        Application.LoadLevel("GameLobby");
    }

    void Refresh() {
        Debug.Log("REFRESH LIST");
    }

    void Menu() {
        Debug.Log("MAIN MENU");
        Application.LoadLevel("MainMenu");
    }
}
