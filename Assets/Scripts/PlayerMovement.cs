using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	#region moveVars
	private Rigidbody myRB;
	//public float moveSpeed = 1.0f;
	//private float sphereRadius;
	#endregion

	#region lookVars
	public float jumpForce = 200f;
	public float mouseSensitivity;
	public Vector2 VerticalLookConstraints;
	public float beginningHorizontalLookRotation;
	private float horizontalLookRotation;
	private float verticalLookRotation;
	public Transform myCamera;
	#endregion

	private float GroundDistance = 1.0f;


	// Use this for initialization
	void Start() {
		horizontalLookRotation = beginningHorizontalLookRotation;
		myRB = this.GetComponent<Rigidbody>();
		if(myCamera == null) {
			myCamera = FindObjectOfType<Camera>().transform;
		}
		if(myCamera == null) {
			Debug.LogError("There is no camera!");
		}
		//sphereRadius = this.transform.localScale.x / 2;

	}

	// Update is called once per frame
	void Update() 
	{
		SetLookRotations();


		//Debug.Log(IsGrounded());
		if(IsGrounded())
		{
			if(Input.GetButtonDown("Jump")) {
				myRB.AddForce(0, jumpForce, 0);
			}
		}

	}

	void SetLookRotations() 
	{

		//Debug.Log(Input.GetAxis("Mouse X"));
		horizontalLookRotation += Input.GetAxis("Mouse X") * mouseSensitivity;
		verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivity;
		verticalLookRotation = Mathf.Clamp(verticalLookRotation, VerticalLookConstraints.x, VerticalLookConstraints.y);
		myCamera.rotation = Quaternion.Euler((Vector3.left * verticalLookRotation) + (Vector3.up * horizontalLookRotation));
	}


	bool IsGrounded()
	{
		return Physics.Raycast (transform.position, - Vector3.up, GroundDistance);
	}
}