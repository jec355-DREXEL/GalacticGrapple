using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ThrusterController : MonoBehaviour 
{
    //ADD RELATIVE FORCE
	private Rigidbody myRB;

	public float ThrustForce = 600.0f;
	private bool playerGrappling = false;
	public Text countText1;
	private int ThrusterCount;
    private float cooldown= 0f;
	private GameObject thePlayer;
	private PlayerControllerTest playerScript;
    public AudioClip thrust;
    public AudioSource audio;

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
		ThrusterCount = playerScript.ThrustCount;
		//Debug.Log (playerGrappling);
		if (ThrusterCount > 0) 
		{
			if (cooldown <= 0) 
			{
                Vector2 input = new Vector2(Input.GetAxis("Horizontal_Thrusters"), Input.GetAxis("Vertical_Thrusters"));
				//	if (Input.GetButtonDown ("Horizontal_Thrusters")) {
				if (input.x > 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddRelativeForce (ThrustForce * input.x* 10, ThrustForce*input.x*10, 0);
					cooldown = 5f;
					ThrusterCount--;
					playerScript.ThrustCount = ThrusterCount;
                    audio.PlayOneShot(thrust);
                } else if (Input.GetAxis ("Horizontal_Thrusters") < 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddRelativeForce (ThrustForce * input.x*10, ThrustForce * input.x*10, 0);
					cooldown = 5f;
					ThrusterCount--;
					playerScript.ThrustCount = ThrusterCount;
                    audio.PlayOneShot(thrust);
                }
				//	}

				//if (Input.GetButtonDown ("Vertical_Thrusters")) {
				if (Input.GetAxis ("Vertical_Thrusters") > 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddRelativeForce (0, ThrustForce*input.y*10, ThrustForce* input.y*10);
					cooldown = 5f;
					ThrusterCount--;
					playerScript.ThrustCount = ThrusterCount;
                    audio.PlayOneShot(thrust);
                } else if (Input.GetAxis ("Vertical_Thrusters") < 0) {
					//myRB.velocity = Vector3.zero;
					//myRB.angularVelocity = Vector3.zero;
					myRB.AddRelativeForce (0, ThrustForce * input.y*10, ThrustForce * input.y*10);
					cooldown = 5f;
					ThrusterCount--;
					playerScript.ThrustCount = ThrusterCount;
                    audio.PlayOneShot(thrust);
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
        //Debug.Log (playerScript.ThrustForce);
	}
}
