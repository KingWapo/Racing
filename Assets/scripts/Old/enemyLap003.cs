using UnityEngine;
using System.Collections;

public class enemyLap003 : MonoBehaviour
{

    public static int lap;
    public GameObject finishLine;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == finishLine)
        {
            lap += 1;
        }
    }
}
