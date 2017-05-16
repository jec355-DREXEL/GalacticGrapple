using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class DriverPlayerGadgetController : MonoBehaviour
{

	#region grappleVars
	ConfigurableJoint myJoint;
	public float maxGrappleDist = 30f;
	public float winchSpeed = 0.5f;
	public float winchMomentumGain = 100f;
	public float hitRadius = 1.25f;
	private bool grappleOn = false;
	public LayerMask grappleMask;
	public LineRenderer myLR = null;
	private SoftJointLimit jointLimit = new SoftJointLimit();
	#endregion

	#region miscVars
	public Camera myCam = null;
	private Rigidbody myRB;

	public float moveSpeed = 1.0f;
	public float maxVelocity = 10.0f;

	private float GroundDistance = 1.0f;
	#endregion

	private void Start() 
	{
		myJoint = this.GetComponent<ConfigurableJoint>();
		if(myLR == null) 
		{
			Debug.LogWarning("No linerenderer set, attempting to find on one the gameobject");
			this.GetComponent<LineRenderer>();
		}
		if(myCam == null) 
		{
			FindObjectOfType<Camera>();
		}
		myRB = this.GetComponent<Rigidbody>();
		myLR.enabled = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update() 
	{
		DoGrappleStuff();
	}

	void DoGrappleStuff() 
	{
		if (grappleOn) {
			myLR.SetPosition (0, myLR.transform.position);
			#region Winch Stuff
			if (Input.GetAxisRaw ("Pull/Push") > 0) 
			{
				//Debug.Log ("Pull");
				jointLimit.limit -= winchSpeed * Time.deltaTime;
				if (jointLimit.limit < .5) 
				{
					jointLimit.limit = 0.5f;
				}
				myJoint.linearLimit = jointLimit;
				Vector3 thingy = myJoint.connectedAnchor - this.transform.position;
				thingy.Normalize ();
				myRB.AddForceAtPosition (thingy * winchMomentumGain * Time.deltaTime, thingy * (-this.transform.localScale.x / 5), ForceMode.Acceleration);
			} else if (Input.GetAxisRaw ("Pull/Push") < 0) 
			{
				//Debug.Log ("Push");
				jointLimit.limit += winchSpeed * Time.deltaTime;
				if (jointLimit.limit > maxGrappleDist) 
				{
					jointLimit.limit = maxGrappleDist;
				}
				myJoint.linearLimit = jointLimit;
			}
			#endregion

			/*
			if(Input.GetButtonDown("Jump")){
				Debug.Log("Space");
				myRB.AddForce(transform.forward * 100);
			}
			*/
		} 
		else 
		{
			//myRB.constraints = RigidbodyConstraints.None;
			myRB.freezeRotation = false;
			Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			if(input.x > 0) 
			{
				//myRB.AddForceAtPosition(myCamera.transform.right * moveSpeed * Time.deltaTime, (-myCamera.transform.right + this.transform.position) * sphereRadius, ForceMode.Acceleration);
				if(myRB.velocity.magnitude < maxVelocity)
					myRB.AddForce(myCam.transform.right * moveSpeed);
			}
			if(input.x < 0) 
			{
				//myRB.AddForceAtPosition(-myCamera.transform.right * moveSpeed * Time.deltaTime, (-myCamera.transform.right + this.transform.position) * sphereRadius, ForceMode.Acceleration);
				if(myRB.velocity.magnitude < maxVelocity)
					myRB.AddForce(-myCam.transform.right * moveSpeed);
			}
			if(input.y > 0) 
			{
				//myRB.AddForceAtPosition(myCamera.transform.forward * moveSpeed * Time.deltaTime, (-myCamera.transform.forward + this.transform.position) * sphereRadius, ForceMode.Acceleration);
				if(myRB.velocity.magnitude < maxVelocity)
					myRB.AddForce(myCam.transform.forward * moveSpeed);
			}
			if(input.y < 0) 
			{
				//myRB.AddForceAtPosition(-myCamera.transform.forward * moveSpeed * Time.deltaTime, (myCamera.transform.forward + this.transform.position) * sphereRadius, ForceMode.Acceleration);
				if(myRB.velocity.magnitude < maxVelocity)
					myRB.AddForce(-myCam.transform.forward * moveSpeed);
			}

			if(Input.GetButtonDown("Fire2")) 
			{
				RaycastHit hit;


				if(Physics.Raycast(myCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f)), out hit, maxGrappleDist)) 
				{
					int layerTrash = 1 << hit.collider.gameObject.layer;
					if((layerTrash & grappleMask.value) != 0) 
					{
						//myLR.SetPosition(1, hit.point);
						myRB.AddForce(myCam.transform.forward * 2000.0f);
					}
				}
			}

			//Debug.Log (IsGrounded());

			if (IsGrounded ()) {
				if (Input.GetButtonDown ("BoostJump")) {
					Debug.Log ("BoostJump");
					myRB.AddForce (0, 600000.0f, 0);
					myRB.freezeRotation = true;
				}
			} else {
				myRB.freezeRotation = false;
			}
		}


        #region Grappling Hook Stuff
		if(Input.GetButtonDown("Fire1")) 
		{
			RaycastHit hit;
			if(!grappleOn) 
			{
				if(Physics.Raycast(myCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f)), out hit, maxGrappleDist)) 
				{
					int layerTrash = 1 << hit.collider.gameObject.layer;
					if((layerTrash & grappleMask.value) != 0) 
					{
						//Debug.Log(hit.point);
						MakeGrappleHook(hit.point);
					}
				} 
				else if(Physics.SphereCast(myCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), hitRadius, out hit, maxGrappleDist)) 
				{
					int layerTrash = 1 << hit.collider.gameObject.layer;
					if((layerTrash & grappleMask.value) != 0) 
					{
						MakeGrappleHook(hit.point);
					}
				}
			} 
			else 
			{
				grappleOn = false;
				jointLimit.limit = Mathf.Infinity;
				myJoint.linearLimit = jointLimit;
				myLR.enabled = false;
			}
		}
		#endregion
	}

	void MakeGrappleHook(Vector3 point) 
	{
		grappleOn = true;
		//myRB.constraints = RigidbodyConstraints.FreezeRotationX;
		myRB.freezeRotation = true;
		myJoint.connectedAnchor = point;
		jointLimit.limit = (this.transform.position - point).magnitude;
		Debug.Log ((this.transform.position - point).magnitude);
		myJoint.linearLimit = jointLimit;
		myLR.enabled = true;
		myLR.SetPosition(1, point);
	}

	bool IsGrounded()
	{
		return Physics.Raycast (transform.position, - Vector3.up, GroundDistance);
	}
}