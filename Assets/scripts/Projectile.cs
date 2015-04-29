using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    float velocity = 4f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * velocity);
	}
}
