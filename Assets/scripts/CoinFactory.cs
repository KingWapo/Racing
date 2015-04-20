using UnityEngine;
using System.Collections;

public class CoinFactory : MonoBehaviour {

    public GameObject CoinPrefab;

    private float summonX = 10;
    private float summonZ =  10;

    private float cooldown = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (cooldown <= 0)
        {
            spawnCoin();
            cooldown = 50;
        }
        else
        {
            cooldown--;
        }
	}

    private void spawnCoin()
    {
        float x = transform.position.x + Random.Range(-summonX, summonX);
        float z = transform.position.z + Random.Range(-summonZ, summonZ);
        Vector3 pos = new Vector3(x, transform.position.y, z);
        Network.Instantiate(CoinPrefab, pos, Quaternion.identity, 0);
    }
}
