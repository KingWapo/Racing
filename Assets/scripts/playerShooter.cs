using UnityEngine;
using System.Collections;

public class playerShooter : MonoBehaviour {

    public float trackSpeed = 10f;

    private float sensitivity = .5f;

    private Transform gunForward;

    private Vector3 trackCenter;
    private float trackRadius = 50f;
    private float rotationAngle;
    private float verticalOffset;

    // Synchronization values
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
    private Quaternion syncStartRotation = Quaternion.identity;

    private networkManager networkManager;

	// Use this for initialization
	void Start () {
        Transform railGun = transform.Find("RailGun");
        railGun.Translate(Mathf.Cos(rotationAngle) * -trackRadius, Mathf.Sin(rotationAngle) * trackRadius, 0);
        gunForward = railGun.FindChild("Gun");

        networkManager = GameObject.Find("GameManager").GetComponent<networkManager>();
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<NetworkView>().isMine) {
            // player update
        } else {
            SyncedMovement();
        }
	}

    public void UpdateMovement(float lAxisX, float rAxisX, float rAxisY) {
        if (Mathf.Abs(lAxisX) > .1f) {
            float radiusMod = Mathf.Sign(lAxisX) * (Mathf.Exp(2 * Mathf.Pow(lAxisX, 2)) - 1);

            float oldRotation = rotationAngle;

            rotationAngle = (rotationAngle + radiusMod) % 360f;
            rotationAngle = rotationAngle < 0f ? rotationAngle + 360f : rotationAngle;

            transform.Rotate(Vector3.up, rotationAngle - oldRotation);
        }

        float rotBound = 45f;

        if (Mathf.Abs(rAxisX) > .1f) {
            Vector3 rotation = gunForward.localRotation.eulerAngles;
            rotation.y += rAxisX;
            if (rotation.y > rotBound && rotation.y < 180) {
                rotation.y = rotBound;
            }

            if (rotation.y < 360f - rotBound && rotation.y > 180) {
                rotation.y = 360f - rotBound;
            }

            gunForward.localEulerAngles = rotation;
        }

        if (Mathf.Abs(rAxisY) > .1f) {
            Vector3 rotation = gunForward.localRotation.eulerAngles;
            rotation.z -= rAxisY;

            rotation.z = Mathf.Clamp(rotation.z, 280f, 359f);
            gunForward.localEulerAngles = rotation;
        }
    }

    public void Shoot(float rTrigger) {
        if (rTrigger >= .9f) {
            RaycastHit hit;

            if (Physics.Raycast(gunForward.position, gunForward.right, out hit)) {
                if (hit.collider.gameObject.tag.Equals("Racer")) {
                    Debug.Log("HIT A CAR!!!");
                    hit.collider.GetComponent<playerRacer>().TeleportToStart();
                }
            }
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;
        Quaternion syncGunForward = Quaternion.identity;
        Quaternion syncRotation = Quaternion.identity;

        if (stream.isWriting) {
            syncPosition = transform.position;
            stream.Serialize(ref syncPosition);

            // = rigidbody.velocity;
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

            syncStartPosition = transform.position;
            syncEndPosition = syncPosition + syncVelocity * syncDelay;

            syncStartRotation = syncRotation;

            gunForward.rotation = syncGunForward;
        }
    }

    private void SyncedMovement() {
        syncTime += Time.deltaTime;
        transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
        transform.rotation = syncStartRotation;
    }
}
