using UnityEngine;
using System.Collections;

public class CoinFactory : MonoBehaviour {

    public GameObject CoinPrefab;

    private bool started;
    private float summonX = 120;
    private float summonZ =  80;
    private GameObject SummonPlane;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (started)
        {
            spawnCoin();
        }
        else
        {
            SummonPlane = GameObject.FindGameObjectWithTag("SummonPlane");
            if (SummonPlane)
            {
                started = true;
            }
        }
	}

    private void spawnCoin()
    {
        float x = SummonPlane.transform.position.x + Random.Range(-summonX, summonX);
        float z = SummonPlane.transform.position.z + Random.Range(-summonZ, summonZ);
        x = x > 0 ? Mathf.Clamp(x, 20, summonX) : Mathf.Clamp(x, -summonX, -20);
        z = z > 0 ? Mathf.Clamp(z, 15, summonZ) : Mathf.Clamp(z, -summonZ, -15);
        Vector3 pos = new Vector3(x, Random.Range(10, 20), z);
        Network.Instantiate(CoinPrefab, pos, Quaternion.identity, 0);
    }
}
