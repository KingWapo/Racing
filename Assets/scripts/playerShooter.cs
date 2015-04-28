using UnityEngine;
using System.Collections;

public class playerShooter : MonoBehaviour {

    private float sensitivity = .5f;

    public GameObject goBarrel;
    public GameObject goRing;

    private Transform barrel;
    private Transform ring;

    // Synchronization values
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    private networkManager networkManager;

	// Use this for initialization
	void Start () {
        ring = goRing.transform;
        barrel = goBarrel.transform;
	}
	
	// Update is called once per frame
	void Update () {
        if (GetComponent<NetworkView>().isMine) {
            // player update
        } else {
            SyncedMovement();
        }
	}

    public void UpdateMovement(float rAxisX, float rAxisY) {

        if (Mathf.Abs(rAxisX) > .1f) {
            Vector3 rotation = ring.localRotation.eulerAngles;
            rotation.z += rAxisX;

            if (rotation.z >= 360)
                rotation.z -= 360;
            else if (rotation.z < 0)
                rotation.z += 360;

            ring.localEulerAngles = rotation;
        }

        if (Mathf.Abs(rAxisY) > .1f) {
            Vector3 rotation = barrel.localRotation.eulerAngles;
            rotation.y += rAxisY;

            rotation.y = Mathf.Clamp(rotation.y, 0f, 60f);
            barrel.localEulerAngles = rotation;
        }
    }

    public void Shoot(float rTrigger) {
        if (rTrigger >= .9f) {
            RaycastHit hit;

            if (Physics.Raycast(barrel.position, barrel.right, out hit)) {
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
        Quaternion syncBarrelRotation = Quaternion.identity;
        Quaternion syncRingRotation = Quaternion.identity;

        if (stream.isWriting) {
            syncPosition = transform.position;
            stream.Serialize(ref syncPosition);

            // = rigidbody.velocity;
            stream.Serialize(ref syncVelocity);

            syncBarrelRotation = goBarrel.transform.rotation;
            stream.Serialize(ref syncBarrelRotation);

            syncRingRotation = goRing.transform.rotation;
            stream.Serialize(ref syncRingRotation);
        } else {
            stream.Serialize(ref syncPosition);
            stream.Serialize(ref syncVelocity);
            stream.Serialize(ref syncBarrelRotation);
            stream.Serialize(ref syncRingRotation);

            syncTime = 0f;
            syncDelay = Time.time - lastSynchronizationTime;
            lastSynchronizationTime = Time.time;

            syncStartPosition = transform.position;
            syncEndPosition = syncPosition + syncVelocity * syncDelay;

            goBarrel.transform.rotation = syncBarrelRotation;

            goRing.transform.rotation = syncRingRotation;
        }
    }

    private void SyncedMovement() {
        syncTime += Time.deltaTime;
        transform.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
    }
}
