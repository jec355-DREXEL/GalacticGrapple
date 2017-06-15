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

	public Image P1_Reticle_Inner;
	public Image P1_Reticle_Timer;
	public Image P1_Reticle_Green;
	public Image P1_Reticle_Yellow;
	public Image P1_Reticle_Orange;
	public Image P1_Reticle_Purple;

	public Image P2_Reticle_Inner;
	public Image P2_Reticle_Timer;
	public Image P2_Reticle_Green;
	public Image P2_Reticle_Yellow;
	public Image P2_Reticle_Orange;
	public Image P2_Reticle_Purple;


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

	public Sprite Player1ReticleEmpty;
	public Sprite Player1ReticleLockOn;
	public Sprite Player1ReticleMining;

	public Sprite Player2ReticleEmpty;
	public Sprite Player2ReticleLockOn;
	public Sprite Player2ReticleMining;

    public AudioSource audio;
    public AudioClip mineAudio;
    public AudioClip grappleOnAudio;
    public AudioClip grappleOffAudio;
    public AudioClip endAudio;
    public bool readyToGo = false;
	#endregion


	public float timeLeft = 270.0f;
	private float starttime;


	private void Start() 
	{
		myTransform = GetComponent<Transform>();
		myJoint = this.GetComponent<ConfigurableJoint>();

		if (myCam == null) {
			FindObjectOfType<Camera>();
		}
		myRB = this.GetComponent<Rigidbody>();

		myLR_P1.enabled = false;
		myLR_P2.enabled = false;
		myLR_Mining.enabled = false;

		//Starting Position of Player
		startPosition = transform.position.z;
		//Starting Time
		starttime = timeLeft;

		//Player1 Reticles
		P1_Reticle_Green.enabled = false;
		P1_Reticle_Yellow.enabled = false;
		P1_Reticle_Orange.enabled = false;
		P1_Reticle_Purple.enabled = false;
		//Player2 Reticles
		P2_Reticle_Green.enabled = false;
		P2_Reticle_Yellow.enabled = false;
		P2_Reticle_Orange.enabled = false;
		P2_Reticle_Purple.enabled = false;

		//SetCountText ();
	}

	private void Update() 
	{
        if (readyToGo) {
            CameraLook();
            DoGrappleStuff();
            GrappleSlack();

            distanceTraveled = transform.position.z - startPosition;

            //SetCountText ();

            //Debug.Log (myRB.velocity.magnitude);


            timeLeft -= Time.deltaTime;

            P1_Reticle_Timer.GetComponent<Image>().fillAmount = timeLeft / starttime;
            P1_Reticle_Timer.GetComponent<Image>().fillAmount = timeLeft / starttime;
            P2_Reticle_Timer.GetComponent<Image>().fillAmount = timeLeft / starttime;
            P2_Reticle_Timer.GetComponent<Image>().fillAmount = timeLeft / starttime;

            Debug.Log(timeLeft);
            if (timeLeft <= 0) {
                readyToGo = false;
            }
        }else {
            //OPERATOR PRESSES ENTER TO ALLOW THE GAME TO PLAY
            if (Input.GetKeyDown(KeyCode.Return)&&timeLeft>0) {
                readyToGo = true;
            }
        }

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



		//--------------------------PLAYER 1 GRAPPLE--------------------------
		Vector2 player1_Input = new Vector2 (Input.GetAxisRaw ("Horizontal_P1"), Input.GetAxisRaw ("Vertical_P1"));

		//Player 1 reticle movement
		Vector2 temp = P1_Reticle_Inner.transform.position;
		if ((P1_Reticle_Inner.transform.position.x + hitRadius < 1260 && player1_Input.x>0)) {
			temp.x += player1_Input.x * crosshairSpeed * Time.deltaTime;     
		}else if((P1_Reticle_Inner.transform.position.x + hitRadius > 20 && player1_Input.x < 0)) {
			temp.x += player1_Input.x * crosshairSpeed * Time.deltaTime;
		}
		if ((P1_Reticle_Inner.transform.position.y + hitRadius < 700 && player1_Input.y>0)){
			temp.y += player1_Input.y * crosshairSpeed * Time.deltaTime;
		}
		if(P1_Reticle_Inner.transform.position.y - hitRadius > 20 && player1_Input.y<0) {
			temp.y += player1_Input.y * crosshairSpeed * Time.deltaTime;
		}
		P1_Reticle_Inner.transform.position = temp;
		P1_Reticle_Timer.transform.position = temp;
		P1_Reticle_Green.transform.position = temp;
		P1_Reticle_Yellow.transform.position = temp;
		P1_Reticle_Orange.transform.position = temp;
		P1_Reticle_Purple.transform.position = temp;

		//Shooting Raycast from Player to reticle position
		Ray ray = Camera.main.ScreenPointToRay (P1_Reticle_Inner.GetComponent<RectTransform> ().position);
		Debug.DrawRay (ray.origin, ray.direction, Color.magenta);

		if (Physics.Raycast (ray, out hit, maxGrappleDist)) 
		{
			int layerTrash = 1 << hit.collider.gameObject.layer;

			//If raycast layer is grappleMask, then reticle color = pink
			if (layerTrash == grappleMask.value)
			{
				P1_Reticle_Inner.GetComponent<Image> ().color = new Color32(255, 0, 255, 255);
				P1_Reticle_Timer.GetComponent<Image> ().color = new Color32(255, 0, 255, 255);

			}
			//If raycast layer is miningAsteroidMask, then reticle color = orange
			else if (layerTrash == miningAsteroidMask.value)
			{
				P1_Reticle_Inner.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
				P1_Reticle_Timer.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
			}
		} 
		else if (Physics.SphereCast (ray, hitRadius, out hit, maxGrappleDist)) 
		{
			int layerTrash = 1 << hit.collider.gameObject.layer;

			//If spherecast layer is grappleMask, then reticle color = pink
			if (layerTrash == grappleMask.value) 
			{
				P1_Reticle_Inner.GetComponent<Image> ().color = new Color32 (255, 0, 255, 255);
				P1_Reticle_Timer.GetComponent<Image> ().color = new Color32 (255, 0, 255, 255);
			}
			//If spherecast layer is miningAsteroidMask, then reticle color = orange
			else if (layerTrash == miningAsteroidMask.value)
			{
				P1_Reticle_Inner.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
				P1_Reticle_Timer.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
			}
		} 
		//Else reticle color = red
		else 
		{
			P1_Reticle_Inner.GetComponent<Image> ().color = new Color32 (255, 0, 0, 255);
			P1_Reticle_Timer.GetComponent<Image> ().color = new Color32 (255, 0, 0, 255);
		}


		if (Input.GetButtonDown ("Grapple_P1")) 
		{
			Ray ray_P1 = Camera.main.ScreenPointToRay (P1_Reticle_Inner.GetComponent<RectTransform> ().position);
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
				myRB.AddRelativeForce (300.0f, 0, 0);

            }
		}



		//--------------------------PLAYER 2 GRAPPLE--------------------------
		Vector2 player2_Input = new Vector2 (Input.GetAxisRaw ("Horizontal_P2"), Input.GetAxisRaw ("Vertical_P2"));

		//Player 2 reticle movement
		Vector2 temp2 = P2_Reticle_Inner.transform.position;
		if ((P2_Reticle_Inner.transform.position.x + hitRadius < 1260 && player2_Input.x > 0)) {
			temp2.x += player2_Input.x * crosshairSpeed * Time.deltaTime;
		} else if ((P2_Reticle_Inner.transform.position.x + hitRadius > 20 && player2_Input.x < 0)) {
			temp2.x += player2_Input.x * crosshairSpeed * Time.deltaTime;
		}
		if ((P2_Reticle_Inner.transform.position.y + hitRadius < 700 && player2_Input.y > 0)) {
			temp2.y += player2_Input.y * crosshairSpeed * Time.deltaTime;
		}
		if (P2_Reticle_Inner.transform.position.y - hitRadius > 20 && player2_Input.y < 0) {
			temp2.y += player2_Input.y * crosshairSpeed * Time.deltaTime;
		}
		P2_Reticle_Inner.transform.position = temp2;
		P2_Reticle_Timer.transform.position = temp2;
		P2_Reticle_Green.transform.position = temp2;
		P2_Reticle_Yellow.transform.position = temp2;
		P2_Reticle_Orange.transform.position = temp2;
		P2_Reticle_Purple.transform.position = temp2;

		//Shooting Raycast from Player to reticle position
		Ray ray2 = Camera.main.ScreenPointToRay (P2_Reticle_Inner.GetComponent<RectTransform> ().position);
		Debug.DrawRay (ray2.origin, ray2.direction, Color.cyan);

		if (Physics.Raycast (ray2, out hit, maxGrappleDist)) 
		{
			int layerTrash = 1 << hit.collider.gameObject.layer;

			//If raycast layer is grappleMask, then reticle color = aqua
			if (layerTrash == grappleMask.value)
			{
				P2_Reticle_Inner.GetComponent<Image> ().color = new Color32 (67, 255, 255, 255);
				P2_Reticle_Timer.GetComponent<Image> ().color = new Color32 (67, 255, 255, 255);

			}
			//If raycast layer is miningAsteroidMask, then reticle color = orange
			else if (layerTrash == miningAsteroidMask.value)
			{
				P2_Reticle_Inner.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
				P2_Reticle_Timer.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
			}
		} 
		else if (Physics.SphereCast (ray2, hitRadius, out hit, maxGrappleDist)) 
		{
			int layerTrash = 1 << hit.collider.gameObject.layer;

			//If spherecast layer is grappleMask, then reticle color = aqua
			if (layerTrash == grappleMask.value)
			{
				P2_Reticle_Inner.GetComponent<Image> ().color = new Color32 (67, 255, 255, 255);
				P2_Reticle_Timer.GetComponent<Image> ().color = new Color32 (67, 255, 255, 255);
			}
			//If spherecast layer is miningAsteroidMask, then reticle color = orange
			else if (layerTrash == miningAsteroidMask.value)
			{
				P2_Reticle_Inner.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
				P2_Reticle_Timer.GetComponent<Image> ().color = new Color32 (255, 120, 0, 255);
			}
		} 
		//Else reticle color = blue
		else 
		{
			P2_Reticle_Inner.GetComponent<Image> ().color = new Color32 (0, 0, 255, 255);
			P2_Reticle_Timer.GetComponent<Image> ().color = new Color32 (0, 0, 255, 255);
		}


		if (Input.GetButtonDown ("Grapple_P2")) 
		{
			Ray ray_P2 = Camera.main.ScreenPointToRay (P2_Reticle_Inner.GetComponent<RectTransform> ().position);
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
				myRB.AddRelativeForce (300.0f, 0, 0);
            }
        }
			


		//--------------------------Mining GRAPPLE--------------------------
		//Debug.Log (trigger_down);
		//If both Players are holding button down
        /*
		if (Input.GetButtonDown("Mining_P1") || Input.GetButtonDown ("Mining_P2")) 
		{
				Ray ray_P1 = Camera.main.ScreenPointToRay (P1_Reticle_Inner.GetComponent<RectTransform> ().position);
				Ray ray_P2 = Camera.main.ScreenPointToRay (P2_Reticle_Inner.GetComponent<RectTransform> ().position);

				
					if ((Physics.Raycast (ray_P1, out Grapple_hit, maxGrappleDist)) && (Physics.Raycast (ray_P2, out Grapple_hit, maxGrappleDist))) {
						int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
						//Debug.Log (Grapple_hit.collider.gameObject.layer);
						if (layerTrash == miningAsteroidMask.value) 
						{
							hit.transform.GetComponent<Renderer> ().material.color = Color.green;
							//SetCountText ();
						}
					} else if ((Physics.SphereCast (ray_P1, hitRadius, out Grapple_hit, maxGrappleDist)) && (Physics.SphereCast (ray_P2, hitRadius, out Grapple_hit, maxGrappleDist))) {
						int layerTrash = 1 << Grapple_hit.collider.gameObject.layer;
						//Debug.Log (Grapple_hit.collider.gameObject.layer);
						if (layerTrash == miningAsteroidMask.value) 
						{
							hit.transform.GetComponent<Renderer> ().material.color = Color.green;
							//SetCountText ();
                        }
					}
				
		}*/
	
	}


	//Player 1 grappling function
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

	//Player 2 grappling function
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

	//Mining grappling function
    /*
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
    */
	void GrappleSlack() 
	{
		if (grappleOn)
		{
			jointLimit.limit = (this.transform.position - Grapple_hit.point).magnitude;
			myJoint.linearLimit = jointLimit;
		}
	}

	void CameraLook() {
		RectTransform p1 = P1_Reticle_Inner.rectTransform;
		RectTransform p2 = P2_Reticle_Inner.rectTransform;
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

	//Trigger function when player collides with Pick Up collectibles
	void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag ("Pick Up"))
		{
			other.gameObject.SetActive (false);

			ThrustCount = ThrustCount + 1;
			//SetCountText ();
		}
		if (other.gameObject.CompareTag ("GreenObject"))
		{
			other.gameObject.SetActive (false);
			P1_Reticle_Green.enabled = true;
			P2_Reticle_Green.enabled = true;

			//SetCountText ();
		}
		if (other.gameObject.CompareTag ("YellowObject"))
		{
			other.gameObject.SetActive (false);
			P1_Reticle_Yellow.enabled = true;
			P2_Reticle_Yellow.enabled = true;

			//SetCountText ();
		}
		if (other.gameObject.CompareTag ("OrangeObject"))
		{
			other.gameObject.SetActive (false);
			P1_Reticle_Orange.enabled = true;
			P2_Reticle_Orange.enabled = true;

			//SetCountText ();
		}
		if (other.gameObject.CompareTag ("PurpleObject"))
		{
			other.gameObject.SetActive (false);
			P1_Reticle_Purple.enabled = true;
			P2_Reticle_Purple.enabled = true;

			//SetCountText ();
		}
	}

	//Function that updates HUD
    /*
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
    */
}