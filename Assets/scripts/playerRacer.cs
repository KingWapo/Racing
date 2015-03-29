using UnityEngine;
using System.Collections;

public class playerRacer : MonoBehaviour {

    public float speed = 10f;

    private Rigidbody rigidbody;
    private NavMeshAgent agent;

    private float sensitivity = .5f;

    private float playerRotation = 300f;
    private float playerLean = 0f;

    private float playerVelocity = 0f;

    private float maxForwardVel = 30f;
    private float maxReverseVel = -4f;

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
        agent = GetComponent<NavMeshAgent>();
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
        float turnAxis = Input.GetAxis("Horizontal");
        float acclAxis = Input.GetAxis("360_Triggers");
        Debug.Log("trigger - " + acclAxis);

        if (turnAxis != 0) {
            transform.Rotate(0, turnAxis * Time.deltaTime * playerRotation * sensitivity, 0);

            if (playerLean >= -12 && playerLean <= 12) {
                playerLean -= Mathf.Sign(turnAxis);
            }
        } else {
            if (playerLean > 0) {
                playerLean -= 1;
            } else if (playerLean < 0) {
                playerLean += 1;
            }
        }

        if (acclAxis > .1f || acclAxis < -.1f) {
                playerVelocity = Mathf.Max(maxReverseVel, Mathf.Min(maxForwardVel, playerVelocity + .5f * acclAxis));
        } else {
            playerVelocity *= .9f;
            if (Mathf.Abs(playerVelocity) <= .000f) {
                playerVelocity = 0f;
            }
        }

        transform.Rotate(playerLean, 0, 0);
        transform.Translate(Time.deltaTime * playerVelocity, 0, 0);
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
