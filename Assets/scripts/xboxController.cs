using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class xboxController : MonoBehaviour {

    public float playerAccel;
    public float playerReverse;
    public float playerRotation = 300;
    public float lean;
	
	// Update is called once per frame

    void Start()
    {
        playerAccel = 0;
        lean = 0;
    }
	void Update () {
        UserInput();
	}
    void UserInput()
    {
        if(Input.GetAxis("Horizontal") == 1)
        {
            transform.Rotate(0, Input.GetAxis("Horizontal") * Time.deltaTime * playerRotation, 0);
            if(playerAccel > 0)
            {
                if (lean >= -12)
                {
                    transform.Rotate(0, 0, lean);
                    lean -= 1;
                }
                else if (lean < -12)
                {
                    transform.Rotate(0, 0, lean);
                }
            }
        }

        else if (Input.GetAxis("Horizontal") == -1)
        {
            transform.Rotate(0, Input.GetAxis("Horizontal") * Time.deltaTime * playerRotation, 0);
            if(playerAccel > 0)
            {
                if (lean <= 12)
                {
                    transform.Rotate(0, 0, lean);
                    lean += 1;
                }
                else if(lean > 12)
                {
                    transform.Rotate(0, 0, lean);
                }
            }
        }
        else
        {
                if(lean > 0)
                {
                    lean -= 1;
                    transform.Rotate(0, 0, lean);
                }
                else if(lean < 0)
                {
                    lean += 1;
                    transform.Rotate(0, 0, lean);
                }
                else if (lean == 0)
                {
                    transform.Rotate(0, 0, lean);
                }
        }

      //  print(playerAccel);
      //foward 
        if(Input.GetAxis("360_Triggers") > 0.1)
        {
            if (playerAccel < 30)
            {
                transform.Translate(0, 0, Input.GetAxis("360_Triggers") * Time.deltaTime * playerAccel);
                playerAccel += .5f;
            }
            if(playerAccel >= 30)
            {
                transform.Translate(0,0, Input.GetAxis("360_Triggers") *Time.deltaTime  * playerAccel);
            }
        }
      //backward
        if(Input.GetAxis("360_Triggers") < -0.1)
        {
            transform.Translate(0, 0, Time.deltaTime * playerAccel);
            playerAccel -= .25f;
            if(playerAccel <= 0)
            {
                playerAccel = 0;
                if(Input.GetAxis("360_Triggers") <-0.1)
                {
                    if(playerReverse < 4)
                    {
                        transform.Translate(0, 0, Input.GetAxis("360_Triggers") * Time.deltaTime * playerReverse);
                        playerReverse += .5f;
                    }
                    else if(playerReverse >=4)
                    {
                        transform.Translate(0, 0, Input.GetAxis("360_Triggers") * Time.deltaTime * playerReverse);
                    }
                }
                else
                {
                    playerReverse = 0;
                }
            }

        }
         
    //slowing down
        else
        {
            transform.Translate(0, 0, Time.deltaTime * playerAccel);
            playerAccel -= .025f;
            if(playerAccel <= 0)
            {
                playerAccel = 0;
            }
        }
        
    }
}
