using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterController : MonoBehaviour 
{
    //ADD RELATIVE FORCE
	private Rigidbody myRB;

	public float thrustForce = 300.0f;
	public bool playerGrappling = false;
    private float cooldown= 0f;
	private GameObject thePlayer;
	private PlayerController playerScript;

	// Use this for initialization
	void Start () 
	{
		myRB = this.GetComponent<Rigidbody>();

		thePlayer = GameObject.Find("Player");
		playerScript = thePlayer.GetComponent<PlayerController>();

	}
	
	// Update is called once per frame
	void Update ()
	{
		playerGrappling = playerScript.grappleOn;
		//Debug.Log (playerGrappling);
        
		if (cooldown<=0) {
		
		//	if (Input.GetButtonDown ("Horizontal_Thrusters")) {
				if (Input.GetAxis ("Horizontal_Thrusters") > 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddRelativeForce (thrustForce, thrustForce, 0);
                    cooldown = 5f;
				} else if (Input.GetAxis("Horizontal_Thrusters") < 0){
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddRelativeForce(-thrustForce, thrustForce, 0);
                    cooldown = 5f;
            }
		//	}

			//if (Input.GetButtonDown ("Vertical_Thrusters")) {
				if (Input.GetAxis ("Vertical_Thrusters") > 0) {
                    //myRB.velocity = Vector3.zero;
                    //myRB.angularVelocity = Vector3.zero;
                    myRB.AddRelativeForce(0, thrustForce, thrustForce);
                    cooldown = 5f;
            } else if (Input.GetAxis("Vertical_Thrusters") < 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddRelativeForce(0, thrustForce, -thrustForce);
                cooldown = 5f;
            }
            //}

        }else {
            cooldown -= Time.deltaTime;
        }
	}
}
