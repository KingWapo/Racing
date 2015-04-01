using UnityEngine;
using System.Collections;

public class playerRacer : MonoBehaviour {

    public float speed = 10f;

    private new Rigidbody rigidbody;

    //public Controller controller;

    // Synchronization values
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
    private Quaternion syncStartRotation = Quaternion.identity;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();

        if (GetComponent<NetworkView>().isMine) {
            Camera.main.transform.parent = transform;

            Camera.main.transform.localPosition = new Vector3(-3, 1, 0);
            //Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(15, 90, 0));
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<NetworkView>().isMine) {
            //controller.UpdateMovement();
            //InputMovement();
        } else {
            SyncedMovement();
        }
	}

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;
        Quaternion syncRotation = Quaternion.identity;

        if (stream.isWriting) {
            syncPosition = transform.position;
            stream.Serialize(ref syncPosition);

            syncVelocity = rigidbody.velocity;
            stream.Serialize(ref syncVelocity);

            syncRotation = transform.rotation;
            stream.Serialize(ref syncRotation);
        } else {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            stream.Serialize(ref syncRotation);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncStartPosition = rigidbody.position;
            syncEndPosition = syncPosition + syncVelocity * syncDelay;

            syncStartRotation = syncRotation;
        }
    }

    private void SyncedMovement() {
        syncTime += Time.deltaTime;
        rigidbody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
        transform.rotation = syncStartRotation;
    }
}
