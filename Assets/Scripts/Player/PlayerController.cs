using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

class PlayerController : MonoBehaviour {

    #region grappleVars
    ConfigurableJoint myJoint;
    public float maxGrappleDist = 50f;
    public float winchSpeed = 5f;
    public float winchMomentumGain = 100f;
    public float hitRadius = 1.25f;
    public bool grappleOn = false;
    public LayerMask grappleMask;
    public LineRenderer myLR = null;
    private SoftJointLimit jointLimit = new SoftJointLimit();
    private RaycastHit hit;
	Ray ray;
    #endregion

    #region miscVars
    public Camera myCam = null;
    private Rigidbody myRB;

	public Image player1_crosshair;
	public Image player2_crosshair;
    public float crosshairSpeed = 100f;
    #endregion

    private void Start() {
        myJoint = this.GetComponent<ConfigurableJoint>();
        if (myLR == null) {
            Debug.LogWarning("No linerenderer set, attempting to find on one the gameobject");
            this.GetComponent<LineRenderer>();
        }
        if (myCam == null) {
            FindObjectOfType<Camera>();
        }
        myRB = this.GetComponent<Rigidbody>();
        myLR.enabled = false;
    }

    private void Update() 
	{
        DoGrappleStuff();
    }

    void DoGrappleStuff() 
	{
        if (grappleOn) 
		{
            myLR.SetPosition(0, myLR.transform.position);
            #region Winch Stuff
			if (Input.GetButtonDown("Pull_P1")) 
			{
				Debug.Log(Input.GetButtonDown("Pull_P1"));
                jointLimit.limit -= winchSpeed * Time.deltaTime;
                if (jointLimit.limit < .5) 
				{
                    jointLimit.limit = 0.5f;
                }
                myJoint.linearLimit = jointLimit;
                Vector3 thingy = myJoint.connectedAnchor - this.transform.position;
                thingy.Normalize();
                myRB.AddForceAtPosition(thingy * winchMomentumGain * Time.deltaTime, thingy * (-this.transform.localScale.x / 5), ForceMode.Acceleration);
			} 
			if (Input.GetButtonDown("Push_P1")) 
			{
                //Debug.Log("Push");
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


		//PLAYER 1 GRAPPLE
		Vector2 player1_Input = new Vector2(Input.GetAxisRaw("Horizontal_P1"), Input.GetAxisRaw("Vertical_P1"));

		Vector2 temp = player1_crosshair.transform.position;
		temp.x += player1_Input.x * crosshairSpeed * Time.deltaTime;
		temp.y += player1_Input.y * crosshairSpeed * Time.deltaTime;
		player1_crosshair.transform.position = temp;

		Ray ray = Camera.main.ScreenPointToRay (player1_crosshair.GetComponent<RectTransform>().position);
		Debug.DrawRay(ray.origin,ray.direction,Color.magenta);

		if(Physics.Raycast(ray,out hit ,maxGrappleDist)) 
		{
			int layerTrash = 1 << hit.collider.gameObject.layer;
			if ((layerTrash & grappleMask.value) != 0) 
			{
				player1_crosshair.GetComponent<Image> ().color = new Color (0, 255, 0);
			}
		}
		else if (Physics.SphereCast(ray, hitRadius, out hit, maxGrappleDist)) {
			//Debug.Log("You selected the " + hit.transform.name);
			int layerTrash = 1 << hit.collider.gameObject.layer;
			if ((layerTrash & grappleMask.value) != 0) 
			{
				player1_crosshair.GetComponent<Image>().color = new Color(0, 255, 0);
			}
		}
		else {
			player1_crosshair.GetComponent<Image> ().color = new Color (255, 0, 0);
		}


		if (Input.GetButtonDown("Grapple_P1")) {
			RaycastHit hit_P1;
			Ray ray_P1 = Camera.main.ScreenPointToRay (player1_crosshair.GetComponent<RectTransform>().position);
			if (!grappleOn) {
				if (Physics.Raycast(ray_P1, out hit_P1, maxGrappleDist)) {
					int layerTrash = 1 << hit_P1.collider.gameObject.layer;
					if ((layerTrash & grappleMask.value) != 0) {
						MakeGrappleHook(hit_P1.point);
					}
				} else if (Physics.SphereCast(ray_P1, hitRadius, out hit_P1, maxGrappleDist)) {
					int layerTrash = 1 << hit_P1.collider.gameObject.layer;
					if ((layerTrash & grappleMask.value) != 0) {
						MakeGrappleHook(hit_P1.point);
					}
				}
			} else {
				grappleOn = false;
				jointLimit.limit = Mathf.Infinity;
				myJoint.linearLimit = jointLimit;
				myLR.enabled = false;
				myRB.AddForce (0, 300.0f, 0);
			}
		}



		//PLAYER 2 GRAPPLE
		Vector2 player2_Input = new Vector2(Input.GetAxisRaw("Horizontal_P2"), Input.GetAxisRaw("Vertical_P2"));

		Vector2 temp2 = player2_crosshair.transform.position;
		temp2.x += player2_Input.x * crosshairSpeed * Time.deltaTime;
		temp2.y += player2_Input.y * crosshairSpeed * Time.deltaTime;
		player2_crosshair.transform.position = temp2;

		Ray ray2 = Camera.main.ScreenPointToRay (player2_crosshair.GetComponent<RectTransform>().position);
		Debug.DrawRay(ray2.origin,ray2.direction,Color.magenta);

		if(Physics.Raycast(ray2,out hit ,maxGrappleDist)) 
		{
			int layerTrash = 1 << hit.collider.gameObject.layer;
			if ((layerTrash & grappleMask.value) != 0) 
			{
				player2_crosshair.GetComponent<Image> ().color = new Color (0, 255, 0);
			}
		}
		else if (Physics.SphereCast(ray2, hitRadius, out hit, maxGrappleDist)) {
			//Debug.Log("You selected the " + hit.transform.name);
			int layerTrash = 1 << hit.collider.gameObject.layer;
			if ((layerTrash & grappleMask.value) != 0) 
			{
				player2_crosshair.GetComponent<Image>().color = new Color(0, 255, 0);
			}
		}
		else {
			player2_crosshair.GetComponent<Image> ().color = new Color (0, 0, 255);
		}


		if (Input.GetButtonDown("Grapple_P2")) {
			RaycastHit hit_P2;
			Ray ray_P2 = Camera.main.ScreenPointToRay (player2_crosshair.GetComponent<RectTransform>().position);
			if (!grappleOn) {
				if (Physics.Raycast(ray_P2, out hit_P2, maxGrappleDist)) {
					int layerTrash = 1 << hit_P2.collider.gameObject.layer;
					if ((layerTrash & grappleMask.value) != 0) {
						MakeGrappleHook(hit_P2.point);
					}
				} else if (Physics.SphereCast(ray_P2, hitRadius, out hit_P2, maxGrappleDist)) {
					int layerTrash = 1 << hit_P2.collider.gameObject.layer;
					if ((layerTrash & grappleMask.value) != 0) {
						MakeGrappleHook(hit_P2.point);
					}
				}
			} else {
				grappleOn = false;
				jointLimit.limit = Mathf.Infinity;
				myJoint.linearLimit = jointLimit;
				myLR.enabled = false;
				myRB.AddForce (0, 300.0f, 0);
			}
		}
    }

    void MakeGrappleHook(Vector3 point) {
        grappleOn = true;
        myJoint.connectedAnchor = point;
        jointLimit.limit = (this.transform.position - point).magnitude;
        myJoint.linearLimit = jointLimit;
        myLR.enabled = true;
        myLR.SetPosition(1, point);
    }
}