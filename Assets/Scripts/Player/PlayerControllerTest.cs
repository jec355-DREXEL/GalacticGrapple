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
	public bool grappleOn_Mining = false;
	public bool grappleOn_P1 = false;
	public bool grappleOn_P2 = false;
	private bool trigger_down = false;
	public LayerMask grappleMask;
	public LayerMask miningAsteroidMask;
	public LineRenderer myLR_P1;
	public LineRenderer myLR_P2;
	public LineRenderer myLR_Mining;
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
	public float crosshairSpeed = 750f;
	public Text countText;
	public int ThrustCount = 4;
	public Text MiningcountText;
	private int MiningAsteroidCount = 0;
	public int MiningAsteroidEndCount = 10;
	public Text DistanceTraveledText;
	private float distanceTraveled = 0;
	private float startPosition;
	public Text gameoverText;

    public AudioSource audio;
    public AudioClip mineAudio;
    public AudioClip grappleOnAudio;
    public AudioClip grappleOffAudio;
    public AudioClip endAudio;
	#endregion


	private void Start() {
		myTransform = GetComponent<Transform>();
		myJoint = this.GetComponent<ConfigurableJoint>();

		if (myCam == null) {
			FindObjectOfType<Camera>();
		}
		myRB = this.GetComponent<Rigidbody>();

		myLR_P1.enabled = false;
		myLR_P2.enabled = false;
		myLR_Mining.enabled = false;

		startPosition = transform.position.z;

		SetCountText ();
	}

	private void Update() 
	{
		CameraLook();
		DoGrappleStuff();
		GrappleSlack();

		distanceTraveled = transform.position.z - startPosition;

		SetCountText ();

		//Debug.Log (myRB.velocity.magnitude);
	}

	void DoGrappleStuff() 
	{
		if (grappleOn) {
			//myRB.AddForce(myRB.velocity.normalized * Time.deltaTime * 10);
			if (grappleOn_Mining) 
			{
				myLR_Mining.SetPosition (0, myLR_Mining.transform.position);
			} else if (grappleOn_P1) {
				myLR_P1.SetPosition (0, myLR_P1.transform.position);
			} else if (grappleOn_P2) {
				myLR_P2.SetPosition (0, myLR_P2.transform.position);
			}
		} 
		else 
		{
			myLR_P1.enabled = false;
			myLR_P2.enabled = false;
			myLR_Mining.enabled = false;
		}


		//if (!grappleOn || grappleOn_P2) {


		//PLAYER 1 GRAPPLE
		Vector2 player1_Input = new Vector2 (Input.GetAxisRaw ("Horizontal_P1"), Input.GetAxisRaw ("Vertical_P1"));
		Vector2 temp = player1_crosshair.transform.position;

		if ((player1_crosshair.transform.position.x + hitRadius < 1260 && player1_Input.x>0)) {
			temp.x += player1_Input.x * crosshairSpeed * Time.deltaTime;     
		}else if((player1_crosshair.transform.position.x + hitRadius > 20 && player1_Input.x < 0)) {
			temp.x += player1_Input.x * crosshairSpeed * Time.deltaTime;
		}
		if ((player1_crosshair.transform.position.y + hitRadius < 700 && player1_Input.y>0)){
			temp.y += player1_Input.y * crosshairSpeed * Time.deltaTime;
		}
		if(player1_crosshair.transform.position.y - hitRadius > 20 && player1_Input.y<0) {
			temp.y += player1_Input.y * crosshairSpeed * Time.deltaTime;
		}
		player1_crosshair.transform.position = temp;
		Ray ray = Camera.main.ScreenPointToRay (player1_crosshair.GetComponent<RectTransform> ().position);

		Debug.DrawRay (ray.origin, ray.direction, Color.magenta);

		if (Physics.Raycast (ray, out hit, maxGrappleDist)) 
		{
			//Debug.Log (hit.collider.gameObject.transform.position);
			int layerTrash = 1 << hit.collider.gameObject.layer;
			if (layerTrash == grappleMask.value)
			{
				player1_crosshair.GetComponent<Image> ().color = new Color32(255, 0, 0, 255);
			}
			else if (layerTrash == miningAsteroidMask.value)
			{
				player1_crosshair.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
			}
		} else if (Physics.SphereCast (ray, hitRadius, out hit, maxGrappleDist)) {
			//Debug.Log (hit.collider.gameObject.transform.position);
			//Debug.Log("You selected the " + hit.transform.name);
			int layerTrash = 1 << hit.collider.gameObject.layer;
			if (layerTrash == grappleMask.value) 
			{
				player1_crosshair.GetComponent<Image> ().color = new Color32 (255, 0, 0, 255);
			}
			else if (layerTrash == miningAsteroidMask.value)
			{
				player1_crosshair.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
			}
		} else {
			player1_crosshair.GetComponent<Image> ().color = new Color32 (140, 0, 0, 255);
		}
		//}


		if (Input.GetButtonDown ("Grapple_P1")) 
		{
			Ray ray_P1 = Camera.main.ScreenPointToRay (player1_crosshair.GetComponent<RectTransform> ().position);
			if (!grappleOn_P1) 
			{
				if (Physics.Raycast (ray_P1, out Grapple_hit, maxGrappleDist)) 
				{
					int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
					if (layerTrash == grappleMask.value) 
					{
						P1_ShootGrapple (Grapple_hit.point);
					}
				} else if (Physics.SphereCast (ray_P1, hitRadius, out Grapple_hit, maxGrappleDist)) 
				{
					int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
					if (layerTrash == grappleMask.value) 
					{
						P1_ShootGrapple (Grapple_hit.point);
					}
				}
			} else 
			{
                grappleOn = false;
                audio.PlayOneShot(grappleOffAudio);
             
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
		if ((player2_crosshair.transform.position.x + hitRadius < 1260 && player2_Input.x > 0)) {
			temp2.x += player2_Input.x * crosshairSpeed * Time.deltaTime;
		} else if ((player2_crosshair.transform.position.x + hitRadius > 20 && player2_Input.x < 0)) {
			temp2.x += player2_Input.x * crosshairSpeed * Time.deltaTime;
		}
		if ((player2_crosshair.transform.position.y + hitRadius < 700 && player2_Input.y > 0)) {
			temp2.y += player2_Input.y * crosshairSpeed * Time.deltaTime;
		}
		if (player2_crosshair.transform.position.y - hitRadius > 20 && player2_Input.y < 0) {
			temp2.y += player2_Input.y * crosshairSpeed * Time.deltaTime;
		}
		player2_crosshair.transform.position = temp2;

		Ray ray2 = Camera.main.ScreenPointToRay (player2_crosshair.GetComponent<RectTransform> ().position);
		Debug.DrawRay (ray2.origin, ray2.direction, Color.cyan);

		if (Physics.Raycast (ray2, out hit, maxGrappleDist)) 
		{
			int layerTrash = 1 << hit.collider.gameObject.layer;
			if (layerTrash == grappleMask.value)
			{
				player2_crosshair.GetComponent<Image> ().color = new Color32 (0, 0, 255, 255);
			}
			else if (layerTrash == miningAsteroidMask.value)
			{
				player2_crosshair.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
			}
		} else if (Physics.SphereCast (ray2, hitRadius, out hit, maxGrappleDist)) {
			//Debug.Log("You selected the " + hit.transform.name);
			int layerTrash = 1 << hit.collider.gameObject.layer;
			if (layerTrash == grappleMask.value)
			{
				player2_crosshair.GetComponent<Image> ().color = new Color32 (0, 0, 255, 255);
			}
			else if (layerTrash == miningAsteroidMask.value)
			{
				player2_crosshair.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
			}
		} else 
		{
			player2_crosshair.GetComponent<Image> ().color = new Color32 (0, 0, 140, 255);
		}
		//}


		if (Input.GetButtonDown ("Grapple_P2")) 
		{
			Ray ray_P2 = Camera.main.ScreenPointToRay (player2_crosshair.GetComponent<RectTransform> ().position);
			if (!grappleOn_P2) 
			{
				if (Physics.Raycast (ray_P2, out Grapple_hit, maxGrappleDist)) 
				{
					int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
					if (layerTrash == grappleMask.value) {
						P2_ShootGrapple (Grapple_hit.point);
					}
				} else if (Physics.SphereCast (ray_P2, hitRadius, out Grapple_hit, maxGrappleDist)) 
				{
					int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
					if (layerTrash == grappleMask.value) {
						P2_ShootGrapple (Grapple_hit.point);
					}
				}
			} 
			else 
			{
                grappleOn = false;
                audio.PlayOneShot(grappleOffAudio);
       
				grappleOn_P2 = false;
				jointLimit.limit = Mathf.Infinity;
				myJoint.linearLimit = jointLimit;
				myLR_P2.enabled = false;
				myRB.AddForce (0, 300.0f, 0);
            }
        }
			

		Debug.Log (trigger_down);

		if (Input.GetAxisRaw ("Mining_P1") == 1 && Input.GetAxisRaw ("Mining_P2") == 1) 
		{
			if (trigger_down == false) 
			{
				Ray ray_P1 = Camera.main.ScreenPointToRay (player1_crosshair.GetComponent<RectTransform> ().position);
				Ray ray_P2 = Camera.main.ScreenPointToRay (player2_crosshair.GetComponent<RectTransform> ().position);

				if (!grappleOn_Mining) 
				{
					if ((Physics.Raycast (ray_P1, out Grapple_hit, maxGrappleDist)) && (Physics.Raycast (ray_P2, out Grapple_hit, maxGrappleDist))) {
						int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
						//Debug.Log (Grapple_hit.collider.gameObject.layer);
						if (layerTrash == miningAsteroidMask.value) 
						{
							MiningGrapple (Grapple_hit.point);
							Grapple_hit.collider.gameObject.layer = 8;
							MiningAsteroidCount++;
							hit.transform.GetComponent<Renderer> ().material.color = Color.green;
							SetCountText ();
						}
					} else if ((Physics.SphereCast (ray_P1, hitRadius, out Grapple_hit, maxGrappleDist)) && (Physics.SphereCast (ray_P2, hitRadius, out Grapple_hit, maxGrappleDist))) {
						int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
						//Debug.Log (Grapple_hit.collider.gameObject.layer);
						if (layerTrash == miningAsteroidMask.value) 
						{
							MiningGrapple (Grapple_hit.point);
							Grapple_hit.collider.gameObject.layer = 8;
							MiningAsteroidCount++;
							hit.transform.GetComponent<Renderer> ().material.color = Color.green;
							SetCountText ();
                        }
					}
				}

				trigger_down = true;

			}
		} else 
		{
			if (trigger_down == true && grappleOn_Mining == true) 
			{
				grappleOn = false;
				grappleOn_Mining = false;
				jointLimit.limit = Mathf.Infinity;
				myJoint.linearLimit = jointLimit;
				myLR_Mining.enabled = false;
				myRB.AddForce (0, 300.0f, 0);

				trigger_down = false;
			}
		}

	
	}

	void P1_ShootGrapple(Vector3 point) 
	{
		grappleOn = true;
		grappleOn_P1 = true;
		grappleOn_P2 = false;

		myLR_P1.enabled = true;
		myLR_P2.enabled = false;
		myLR_Mining.enabled = false;

		myJoint.connectedAnchor = point;
		jointLimit.limit = (this.transform.position - point).magnitude;
		myJoint.linearLimit = jointLimit;

		myLR_P1.SetPosition(1, point);
        audio.PlayOneShot(grappleOnAudio);
    }

	void P2_ShootGrapple(Vector3 point) 
	{
		grappleOn = true;
		grappleOn_P1 = false;
		grappleOn_P2 = true;

		myLR_P1.enabled = false;
		myLR_P2.enabled = true;
		myLR_Mining.enabled = false;

		myJoint.connectedAnchor = point;
		jointLimit.limit = (this.transform.position - point).magnitude;
		myJoint.linearLimit = jointLimit;

		myLR_P2.SetPosition(1, point);
        audio.PlayOneShot(grappleOnAudio);
	}

	void MiningGrapple(Vector3 point) 
	{
		grappleOn = true;
		grappleOn_Mining = true;

		myLR_P1.enabled = false;
		myLR_P2.enabled = false;
		myLR_Mining.enabled = true;

		myJoint.connectedAnchor = point;
		jointLimit.limit = (this.transform.position - point).magnitude;
		myJoint.linearLimit = jointLimit;

		myLR_Mining.SetPosition(1, point);
        audio.PlayOneShot(mineAudio);
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

		if ((p1.position.x+hitRadius >1000 && p2.position.x + hitRadius > 1000)) {
			x += 1;
		}else if((p1.position.x - hitRadius < 50 && p2.position.x - hitRadius < 50)) {
			x -= 1;
		}
		if ((p1.position.y - hitRadius < 50 && p2.position.y - hitRadius < 50&& y < 5)) {
			y += 1;
		} else if ((p1.position.y + hitRadius > 740 && p2.position.y + hitRadius > 640&& y > -5)) {
			y -= 1;
		}
		myTransform.Rotate(new Vector3(y, x, 0));
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag ("Pick Up"))
		{
			other.gameObject.SetActive (false);

			ThrustCount = ThrustCount + 1;
			SetCountText ();
		}
	}

	void SetCountText ()
	{
		countText.text = "Thruster Count: " + ThrustCount.ToString();
		MiningcountText.text = "Mining Count: " + MiningAsteroidCount.ToString() + "/" + MiningAsteroidEndCount.ToString();
		DistanceTraveledText.text = "Distance Traveled: " + Mathf.RoundToInt(distanceTraveled).ToString() + " m";

		if(MiningAsteroidCount >= MiningAsteroidEndCount)
		{
			Debug.Log ("GAMEOVER");
			gameoverText.GetComponent<Text> ().enabled = true;
            audio.PlayOneShot(endAudio);
		}
	}
}