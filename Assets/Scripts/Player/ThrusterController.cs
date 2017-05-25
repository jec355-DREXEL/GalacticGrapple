using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ThrusterController : MonoBehaviour 
{
    //ADD RELATIVE FORCE
	private Rigidbody myRB;

	public float ThrustForce = 300.0f;
	private bool playerGrappling = false;
	public Text countText1;
	private int ThrusterCount;
    private float cooldown= 0f;
	private GameObject thePlayer;
	private PlayerControllerTest playerScript;

	// Use this for initialization
	void Start () 
	{
		myRB = this.GetComponent<Rigidbody>();

		thePlayer = GameObject.Find("Player");
		playerScript = thePlayer.GetComponent<PlayerControllerTest>();

	}
	
	// Update is called once per frame
	void Update ()
	{
		playerGrappling = playerScript.grappleOn;
		ThrusterCount = playerScript.ThrustForce;
		//Debug.Log (playerGrappling);
		if (ThrusterCount > 0) 
		{
			if (cooldown <= 0) 
			{
		
				//	if (Input.GetButtonDown ("Horizontal_Thrusters")) {
				if (Input.GetAxis ("Horizontal_Thrusters") > 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddRelativeForce (ThrustForce, ThrustForce, 0);
					cooldown = 5f;
					ThrusterCount--;
					playerScript.ThrustForce = ThrusterCount;
				} else if (Input.GetAxis ("Horizontal_Thrusters") < 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddRelativeForce (-ThrustForce, ThrustForce, 0);
					cooldown = 5f;
					ThrusterCount--;
					playerScript.ThrustForce = ThrusterCount;
				}
				//	}

				//if (Input.GetButtonDown ("Vertical_Thrusters")) {
				if (Input.GetAxis ("Vertical_Thrusters") > 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddRelativeForce (0, ThrustForce, ThrustForce);
					cooldown = 5f;
					ThrusterCount--;
					playerScript.ThrustForce = ThrusterCount;
				} else if (Input.GetAxis ("Vertical_Thrusters") < 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddRelativeForce (0, ThrustForce, -ThrustForce);
					cooldown = 5f;
					ThrusterCount--;
					playerScript.ThrustForce = ThrusterCount;
				}
				//}

			} else {
				cooldown -= Time.deltaTime;
			}
			SetCountText ();
		}
	}

	void SetCountText ()
	{
		countText1.text = "Thruster Count: " + ThrusterCount.ToString ();
		Debug.Log (playerScript.ThrustForce);
	}
}
