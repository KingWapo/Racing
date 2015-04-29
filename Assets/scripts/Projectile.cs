using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    float velocity = 4f;
    Vector3 startPos;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * velocity);

        if (Vector3.Distance(transform.position, startPos) > 500) {
            Destroy(gameObject);
        }
	}
}
