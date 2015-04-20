using UnityEngine;
using System.Collections;

public class SpinCoin : MonoBehaviour {

    private int lifetime = 500;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(new Vector3(0, 1, 0));

        lifetime--;
        if ( lifetime <= 0)
        {
            Network.Destroy(this.gameObject);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DeathBox")
        {
            Network.Destroy(this.gameObject);
        }
    }
}
