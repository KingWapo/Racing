using UnityEngine;
using System.Collections;

public class playerShooter : MonoBehaviour {

    public float trackSpeed = 10f;

    private float sensitivity = .5f;

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
        transform.Find("RailGun").Translate(Mathf.Cos(rotationAngle) * trackRadius, Mathf.Sin(rotationAngle) * trackRadius, 0);
        gunForward = transform.FindChild("Gun");
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
            float radiusMod = Mathf.Sign(lAxisX) * Mathf.Exp(2 * Mathf.Pow(lAxisX, 2)) - 1;

            rotationAngle = (rotationAngle + radiusMod) % 360f;
            rotationAngle = rotationAngle < 0f ? rotationAngle + 360f : rotationAngle;

            transform.rotation.Set(0f, rotationAngle, 0f, 0f);
        }
        
        float theta_scale = 0.1f;             //Set lower to add more points
        int size = (int)((2.0 * Mathf.PI) / theta_scale); //Total number of points in circle.

        /*LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.SetColors(Color.black, Color.red);
        lineRenderer.SetWidth(0.2F, 0.2F);
        lineRenderer.SetVertexCount(size);

        int i = 0;
        for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.1f) {
            float x = trackRadius * Mathf.Cos(theta);
            float y = trackRadius * Mathf.Sin(theta);

            Vector3 pos = new Vector3(x, y, 0);
            lineRenderer.SetPosition(i, pos);
            i += 1;
        }*/
    }

    void OnSerializedNetworkView(BitStream stream, NetworkMessageInfo info) {
        Vector3 syncPosition = Vector3.zero;
        Vector3 syncVelocity = Vector3.zero;
        Quaternion syncGunForward = Quaternion.identity;
        Quaternion syncRotation = Quaternion.identity;

        if (stream.isWriting) {
            //syncPosition = transform.position;
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

    public void InitializeShooterTrack(Vector3 center, float radius, float angle, float offset) {
        trackCenter = center;
        trackRadius = radius;
        rotationAngle = angle;
        verticalOffset = offset;

        trackCenter.y += verticalOffset;
    }
}
