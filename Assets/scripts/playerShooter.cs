using UnityEngine;
using System.Collections;

public class playerShooter : MonoBehaviour {

    public float trackSpeed = 10f;

    private float sensitivity = .5f;

    private new Rigidbody rigidbody;

    private Transform gunForward;

    private Vector3 trackCenter;
    private float trackRadius;
    private float rotationAngle;
    private float verticalOffset;

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
        gunForward = this.transform.FindChild("Gun");
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<NetworkView>().isMine) {
            // player update
        } else {
            SyncedMovement();
        }
	}

    public void UpdateMovement(float lAxisX, float lAxisY, float rAxisX, float rAxisY) {

    }

    void OnSerializedNetworkView(BitStream stream, NetworkMessageInfo info) {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;
        Quaternion syncGunForward = Quaternion.identity;
        Quaternion syncRotation = Quaternion.identity;

        if (stream.isWriting) {
            syncPosition = transform.position;
            stream.Serialize(ref syncPosition);

            syncVelocity = rigidbody.velocity;
            stream.Serialize(ref syncVelocity);

            syncRotation = transform.rotation;
            stream.Serialize(ref syncRotation);

            syncGunForward = gunForward.rotation;
            stream.Serialize(ref syncGunForward);
        } else {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            stream.Serialize(ref syncRotation);
            stream.Serialize(ref syncGunForward);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncStartPosition = rigidbody.position;
            syncEndPosition = syncPosition + syncVelocity * syncDelay;

            syncStartRotation = syncRotation;

            gunForward.rotation = syncGunForward;
        }
    }

    private void SyncedMovement() {
        syncTime += Time.deltaTime;
        rigidbody.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
        transform.rotation = syncStartRotation;
    }

    public void InitializeShooterTrack(Vector3 center, float radius, float angle, float offset) {
        trackCenter = center;
        trackRadius = radius;
        rotationAngle = angle;
        verticalOffset = offset;

        trackCenter.y += verticalOffset;
    }
}
