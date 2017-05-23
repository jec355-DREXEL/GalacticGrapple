using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

class PlayerControllerTest : MonoBehaviour {

    #region grappleVars
    ConfigurableJoint myJoint;
    public float maxGrappleDist = 50f;
    public float winchSpeed = 5f;
    public float winchMomentumGain = 100f;
    public float hitRadius = 1.25f;
    public bool grappleOn = false;
	public bool grappleOn_P1 = false;
	public bool grappleOn_P2 = false;
    public LayerMask grappleMask;
    public LineRenderer myLR_P1 = null;
    public LineRenderer myLR_P2 = null;
    private SoftJointLimit jointLimit = new SoftJointLimit();
    private RaycastHit hit;
	private RaycastHit Grapple_hit;
	Ray ray;
    #endregion

    #region miscVars
    private Transform myTransform = null;
    public Camera myCam = null;
    private Rigidbody myRB;

	public Image player1_crosshair;
	public Image player2_crosshair;
    public float crosshairSpeed = 100f;
    #endregion

    private void Start() {
        myTransform = GetComponent<Transform>();
        myJoint = this.GetComponent<ConfigurableJoint>();
        if (myLR_P1 == null) {
            Debug.LogWarning("No linerenderer set, attempting to find on one the gameobject");
            this.GetComponent<LineRenderer>();
        }
        if (myCam == null) {
            FindObjectOfType<Camera>();
        }
        myRB = this.GetComponent<Rigidbody>();
        myLR_P1.enabled = false;
    }

    private void Update() 
	{
        CameraLook();
        DoGrappleStuff();
		GrappleSlack();
    }

    void DoGrappleStuff() 
	{
        if (grappleOn) 
		{

			if (grappleOn_P1) 
			{
				
				myLR_P1.SetPosition (0, myLR_P1.transform.position);
				#region Winch Stuff
				if (Input.GetButtonDown ("Pull_P1")) 
				{


					jointLimit.limit -= winchSpeed * Time.deltaTime;
					Debug.Log(jointLimit.limit);
					if (jointLimit.limit < .5) 
					{
						jointLimit.limit = 0.5f;
					}
					myJoint.linearLimit = jointLimit;
					Vector3 thingy = myJoint.connectedAnchor - this.transform.position;
					thingy.Normalize ();
					myRB.AddForceAtPosition (thingy * winchMomentumGain * Time.deltaTime, thingy * (-this.transform.localScale.x / 5), ForceMode.Acceleration);
				} 
				if (Input.GetButtonDown ("Push_P1")) 
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


			} 
			else if (grappleOn_P2) 
			{
				myLR_P1.SetPosition (0, myLR_P1.transform.position);
				#region Winch Stuff
				if (Input.GetButtonDown ("Pull_P2")) 
				{
					jointLimit.limit -= winchSpeed * Time.deltaTime;
					if (jointLimit.limit < .5) 
					{
						jointLimit.limit = 0.5f;
					}
					myJoint.linearLimit = jointLimit;
					Vector3 thingy = myJoint.connectedAnchor - this.transform.position;
					thingy.Normalize ();
					myRB.AddForceAtPosition (thingy * winchMomentumGain * Time.deltaTime, thingy * (-this.transform.localScale.x / 5), ForceMode.Acceleration);
				} 
				if (Input.GetButtonDown ("Push_P2")) 
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



			}
        }


		//if (!grappleOn || grappleOn_P2) {
			//PLAYER 1 GRAPPLE
			Vector2 player1_Input = new Vector2 (Input.GetAxisRaw ("Horizontal_P1"), Input.GetAxisRaw ("Vertical_P1"));

			Vector2 temp = player1_crosshair.transform.position;
			

            if ((player1_crosshair.transform.position.x + hitRadius < 1515 && player1_Input.x>0)) {
                temp.x += player1_Input.x * crosshairSpeed * Time.deltaTime;     
            }else if((player1_crosshair.transform.position.x + hitRadius > 20 && player1_Input.x < 0)) {
            temp.x += player1_Input.x * crosshairSpeed * Time.deltaTime;
            }
            if ((player1_crosshair.transform.position.y + hitRadius < 672 && player1_Input.y>0)){
                temp.y += player1_Input.y * crosshairSpeed * Time.deltaTime;
            }
            if(player1_crosshair.transform.position.y - hitRadius > 20 && player1_Input.y<0) {
                temp.y += player1_Input.y * crosshairSpeed * Time.deltaTime;
            }
            player1_crosshair.transform.position = temp;
            Ray ray = Camera.main.ScreenPointToRay (player1_crosshair.GetComponent<RectTransform> ().position);
			Debug.DrawRay (ray.origin, ray.direction, Color.magenta);

			if (Physics.Raycast (ray, out hit, maxGrappleDist)) {
				int layerTrash = 1 << hit.collider.gameObject.layer;
				if ((layerTrash & grappleMask.value) != 0) {
					player1_crosshair.GetComponent<Image> ().color = new Color (0, 255, 0);
				}
			} else if (Physics.SphereCast (ray, hitRadius, out hit, maxGrappleDist)) {
				//Debug.Log("You selected the " + hit.transform.name);
				int layerTrash = 1 << hit.collider.gameObject.layer;
				if ((layerTrash & grappleMask.value) != 0) {
					player1_crosshair.GetComponent<Image> ().color = new Color (0, 255, 0);
				}
			} else {
				player1_crosshair.GetComponent<Image> ().color = new Color (255, 0, 0);
			}
		//}


			if (Input.GetButtonDown ("Grapple_P1")) 
			{
				Ray ray_P1 = Camera.main.ScreenPointToRay (player1_crosshair.GetComponent<RectTransform> ().position);
				if (!grappleOn_P1) {
				    if (Physics.Raycast (ray_P1, out Grapple_hit, maxGrappleDist)) {
					    int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
						    if ((layerTrash & grappleMask.value) != 0) {
							    grappleOn_P1 = true;
							    //myRB.angularVelocity = Vector3.zero;
						        MakeGrappleHook (Grapple_hit.point);
						    }
				        } else if (Physics.SphereCast (ray_P1, hitRadius, out Grapple_hit, maxGrappleDist)) {
					        int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
						    if ((layerTrash & grappleMask.value) != 0) {
							    grappleOn_P1 = true;
							    //myRB.angularVelocity = Vector3.zero;
						        MakeGrappleHook (Grapple_hit.point);
						    }
					    }
				} else {
					grappleOn = false;
                    grappleOn_P1 = false;
					jointLimit.limit = Mathf.Infinity;
					myJoint.linearLimit = jointLimit;
					myLR_P1.enabled = false;
					myRB.AddForce (0, 300.0f, 0);
				}
			}
		


		//if (!grappleOn || grappleOn_P1) {
			//PLAYER 2 GRAPPLE
			Vector2 player2_Input = new Vector2 (Input.GetAxisRaw ("Horizontal_P2"), Input.GetAxisRaw ("Vertical_P2"));

			Vector2 temp2 = player2_crosshair.transform.position;
        if ((player2_crosshair.transform.position.x + hitRadius < 1515 && player2_Input.x > 0)) {
            temp2.x += player2_Input.x * crosshairSpeed * Time.deltaTime;
        } else if ((player2_crosshair.transform.position.x + hitRadius > 20 && player2_Input.x < 0)) {
            temp2.x += player2_Input.x * crosshairSpeed * Time.deltaTime;
        }
        if ((player2_crosshair.transform.position.y + hitRadius < 672 && player2_Input.y > 0)) {
            temp2.y += player2_Input.y * crosshairSpeed * Time.deltaTime;
        }
        if (player2_crosshair.transform.position.y - hitRadius > 20 && player2_Input.y < 0) {
            temp2.y += player2_Input.y * crosshairSpeed * Time.deltaTime;
        }
        player2_crosshair.transform.position = temp2;

			Ray ray2 = Camera.main.ScreenPointToRay (player2_crosshair.GetComponent<RectTransform> ().position);
			Debug.DrawRay (ray2.origin, ray2.direction, Color.cyan);

			if (Physics.Raycast (ray2, out hit, maxGrappleDist)) {
				int layerTrash = 1 << hit.collider.gameObject.layer;
				if ((layerTrash & grappleMask.value) != 0) {
					player2_crosshair.GetComponent<Image> ().color = new Color (0, 255, 0);
				}
			} else if (Physics.SphereCast (ray2, hitRadius, out hit, maxGrappleDist)) {
				//Debug.Log("You selected the " + hit.transform.name);
				int layerTrash = 1 << hit.collider.gameObject.layer;
				if ((layerTrash & grappleMask.value) != 0) {
					player2_crosshair.GetComponent<Image> ().color = new Color (0, 255, 0);
				}
			} else {
				player2_crosshair.GetComponent<Image> ().color = new Color (0, 0, 255);
			}
		//}


			if (Input.GetButtonDown ("Grapple_P2")) 
			{
				Ray ray_P2 = Camera.main.ScreenPointToRay (player2_crosshair.GetComponent<RectTransform> ().position);
				if (!grappleOn_P2) {
				    if (Physics.Raycast (ray_P2, out Grapple_hit, maxGrappleDist)) {
					    int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
						    if ((layerTrash & grappleMask.value) != 0) {
							    grappleOn_P2 = true;
							    //myRB.angularVelocity = Vector3.zero;
						        MakeGrappleHook (Grapple_hit.point);
						    }
				        } else if (Physics.SphereCast (ray_P2, hitRadius, out Grapple_hit, maxGrappleDist)) {
					        int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
							grappleOn_P2 = true;
							//myRB.angularVelocity = Vector3.zero;
						    MakeGrappleHook (Grapple_hit.point);
						}
				}else {
					    grappleOn = false;
                        grappleOn_P2 = false;
                        jointLimit.limit = Mathf.Infinity;
					    myJoint.linearLimit = jointLimit;
					    myLR_P1.enabled = false;
					    myRB.AddForce (0, 300.0f, 0);
		        }
			}	
    }

    void MakeGrappleHook(Vector3 point) 
	{
        grappleOn = true;
        myJoint.connectedAnchor = point;
        jointLimit.limit = (this.transform.position - point).magnitude;
        myJoint.linearLimit = jointLimit;
        myLR_P1.enabled = true;
        myLR_P1.SetPosition(1, point);
    }

	void GrappleSlack() 
	{
		if (grappleOn)
		{
			jointLimit.limit = (this.transform.position - Grapple_hit.point).magnitude;
			myJoint.linearLimit = jointLimit;
		}
	}
    void CameraLook() {
        RectTransform p1 = player1_crosshair.rectTransform;
        RectTransform p2 = player2_crosshair.rectTransform;
        float x = 0f;
        float y = 0;
        Debug.Log(p1.position);
        if ((p1.position.x+hitRadius >1450 && p2.position.x + hitRadius > 1450)) {
            x += 1;
        }else if((p1.position.x - hitRadius < 50 && p2.position.x - hitRadius < 50)) {
            x -= 1;
        }
        if ((p1.position.y - hitRadius < 50 && p2.position.y - hitRadius < 50)) {
            y += 1;
        } else if ((p1.position.y + hitRadius > 600 && p2.position.y + hitRadius > 600)) {
            y -= 1;
        }
        myTransform.Rotate(new Vector3(y, x, 0));
    }
}