using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterController : MonoBehaviour 
{

	private Rigidbody myRB;

	public float thrustForce = 300.0f;
	public bool playerGrappling = false;

	private GameObject thePlayer;
	private PlayerControllerMouse playerScript;

	// Use this for initialization
	void Start () 
	{
		myRB = this.GetComponent<Rigidbody>();

		thePlayer = GameObject.Find("Player");
		playerScript = thePlayer.GetComponent<PlayerControllerMouse>();

	}
	
	// Update is called once per frame
	void Update ()
	{
		playerGrappling = playerScript.grappleOn;
		//Debug.Log (playerGrappling);

		if (!playerGrappling) 
		{
		
			if (Input.GetButtonDown ("Horizontal")) {
				if (Input.GetAxis ("Horizontal") > 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddForce (thrustForce, thrustForce, 0);
				} else {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddForce (-thrustForce, thrustForce, 0);
				}
			}

			if (Input.GetButtonDown ("Vertical")) {
				if (Input.GetAxis ("Vertical") > 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddForce (0, thrustForce, thrustForce);
				} else {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddForce (0, thrustForce, -thrustForce);
				}
			}
		
		}
	}
}
