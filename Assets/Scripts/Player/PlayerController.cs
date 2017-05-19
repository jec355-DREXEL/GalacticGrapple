using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

class PlayerController : MonoBehaviour {

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
    private RaycastHit hit;
    #endregion

    #region miscVars
    public Camera myCam = null;
    private Rigidbody myRB;

    public float moveSpeed = 1.0f;
    public float maxVelocity = 10.0f;
    public Image crosshair;
    public float crosshairSpeed = 100f;
    Ray ray;
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

    private void Update() {
        DoGrappleStuff();
    }

    void DoGrappleStuff() {
        if (grappleOn) {
            myLR.SetPosition(0, myLR.transform.position);
            #region Winch Stuff
            if (Input.GetAxisRaw("Pull/Push") > 0) {
                Debug.Log("Pull");
                jointLimit.limit -= winchSpeed * Time.deltaTime;
                if (jointLimit.limit < .5) {
                    jointLimit.limit = 0.5f;
                }
                myJoint.linearLimit = jointLimit;
                Vector3 thingy = myJoint.connectedAnchor - this.transform.position;
                thingy.Normalize();
                myRB.AddForceAtPosition(thingy * winchMomentumGain * Time.deltaTime, thingy * (-this.transform.localScale.x / 5), ForceMode.Acceleration);
            } else if (Input.GetAxisRaw("Pull/Push") < 0) {
                Debug.Log("Push");
                jointLimit.limit += winchSpeed * Time.deltaTime;
                if (jointLimit.limit > maxGrappleDist) {
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
        } else {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            Vector2 temp = crosshair.transform.position;
            temp.x += input.x * crosshairSpeed * Time.deltaTime;
            temp.y += input.y * crosshairSpeed * Time.deltaTime;
            crosshair.transform.position = temp;

			Ray ray = Camera.main.ScreenPointToRay (crosshair.GetComponent<RectTransform>().position);
			Debug.DrawRay(ray.origin,ray.direction,Color.magenta);

			if(Physics.Raycast(ray,out hit ,maxGrappleDist)) 
			{
                int layerTrash = 1 << hit.collider.gameObject.layer;
				if ((layerTrash & grappleMask.value) != 0) 
				{
					crosshair.GetComponent<Image> ().color = new Color (255, 0, 0);
				}
            }
			else if (Physics.SphereCast(ray, hitRadius, out hit, maxGrappleDist)) {
				//Debug.Log("You selected the " + hit.transform.name);
				int layerTrash = 1 << hit.collider.gameObject.layer;
				if ((layerTrash & grappleMask.value) != 0) 
				{
					crosshair.GetComponent<Image>().color = new Color(255, 0, 0);
				}
			}
			else {
				crosshair.GetComponent<Image> ().color = new Color (0, 0, 0);
			}
        }


        #region Grappling Hook Stuff
        if (Input.GetButtonDown("Fire1")) {
            RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (crosshair.GetComponent<RectTransform>().position);
            if (!grappleOn) {
                if (Physics.Raycast(ray, out hit, maxGrappleDist)) {
                    int layerTrash = 1 << hit.collider.gameObject.layer;
                    if ((layerTrash & grappleMask.value) != 0) {
                        MakeGrappleHook(hit.point);
                    }
                } else if (Physics.SphereCast(ray, hitRadius, out hit, maxGrappleDist)) {
                    int layerTrash = 1 << hit.collider.gameObject.layer;
                    if ((layerTrash & grappleMask.value) != 0) {
                        MakeGrappleHook(hit.point);
                    }
                }
            } else {
                grappleOn = false;
                jointLimit.limit = Mathf.Infinity;
                myJoint.linearLimit = jointLimit;
                myLR.enabled = false;
            }
        }
        #endregion
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