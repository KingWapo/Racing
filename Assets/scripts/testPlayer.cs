using UnityEngine;
using System.Collections;

public class testPlayer : MonoBehaviour {

    public float speed = 10f;

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    private Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<NetworkView>().isMine) {
            InputMovement();
        } else {
            SyncedMovement();
        }
	}

    void InputMovement() {
        if (Input.GetKey(KeyCode.W)) {
            rigidbody.MovePosition(rigidbody.position + Vector3.forward * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A)) {
            rigidbody.MovePosition(rigidbody.position - Vector3.right * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S)) {
            rigidbody.MovePosition(rigidbody.position - Vector3.forward * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) {
            rigidbody.MovePosition(rigidbody.position + Vector3.right * speed * Time.deltaTime);
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;

        if (stream.isWriting) {
            syncPosition = rigidbody.position;
            stream.Serialize(ref syncPosition);

            syncVelocity = rigidbody.velocity;
            stream.Serialize(ref syncVelocity);
        } else {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            //rigidbody.position = syncPosition;

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncStartPosition = rigidbody.position;
            syncEndPosition = syncPosition + syncVelocity * syncDelay;
        }
    }

    private void SyncedMovement() {
        syncTime += Time.deltaTime;
        rigidbody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
    }
}
