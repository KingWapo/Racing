using UnityEngine;
using System.Collections;

public class playerRacer : MonoBehaviour {

    public float speed = 10f;

    private float sensitivity = .5f;

    private float playerRotation = 300f;
    private float playerLean = 0f;
    private float maxPlayerLean = 12f;

    private float playerVelocity = 0f;

    private float maxForwardVel = 30f;
    private float maxReverseVel = -4f;

    private float acceleration = 0f;

    private new Rigidbody rigidbody;
    private NavMeshAgent agent;

    private bool running = true;

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
	}
	
	// Update is called once per frame
	void Update () {
        if (!GetComponent<NetworkView>().isMine) {
            SyncedMovement();
        }
        if (Mathf.Abs(acceleration) <= 0.1f)
        {
            rigidbody.velocity = Vector3.zero;
        }

        if (!running) {
            gameObject.SetActive(running);
        }
	}

    public void UpdateMovement(float turnAxis, float acclAxis) {
        acceleration = acclAxis;
        if (Mathf.Abs(turnAxis) > .1f && Mathf.Abs(acclAxis) > .1f) {
            playerLean = Mathf.Clamp(playerLean - Mathf.Sign(turnAxis), -maxPlayerLean, maxPlayerLean);
        } else {
            if (playerLean > 0f) {
                playerLean -= 1f;
            } else if (playerLean < 0f) {
                playerLean += 1f;
            }
        }

        if (Mathf.Abs(acclAxis) > .1f) {
            if (turnAxis != 0) {
                transform.Rotate(0, turnAxis * Time.deltaTime * playerRotation * sensitivity * Mathf.Sign(playerVelocity), 0);
            }

            playerVelocity = Mathf.Clamp(playerVelocity + .5f * acclAxis, maxReverseVel, maxForwardVel);
        } else {
            playerVelocity *= .9f;
            if (Mathf.Abs(playerVelocity) <= .0001f) {
                playerVelocity = 0f;
            }
        }

        transform.Rotate(0, 0, playerLean);
        transform.Translate(0,0, Time.deltaTime * playerVelocity);
    }

    public float GetLean() {
        return playerLean;
    }

    public void setRunning(bool isRunning) {
        running = isRunning;
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;
        Quaternion syncRotation = Quaternion.identity;
        bool syncRunning = true;

        if (stream.isWriting) {
            syncPosition = transform.position;
            stream.Serialize(ref syncPosition);

            syncVelocity = rigidbody.velocity;
            stream.Serialize(ref syncVelocity);

            syncRotation = transform.rotation;
            stream.Serialize(ref syncRotation);

            syncRunning = running;
            stream.Serialize(ref syncRunning);
        } else {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            stream.Serialize(ref syncRotation);
            stream.Serialize(ref syncRunning);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncStartPosition = rigidbody.position;
            syncEndPosition = syncPosition + syncVelocity * syncDelay;

            syncStartRotation = syncRotation;

            running = syncRunning;
        }
    }

    private void SyncedMovement() {
        syncTime += Time.deltaTime;
        rigidbody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
        transform.rotation = syncStartRotation;
    }
}
