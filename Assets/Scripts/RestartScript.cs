using UnityEngine;
using System.Collections;

public class RestartScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButton("Restart")||Input.GetKeyDown(KeyCode.Backspace))
		{
			//Debug.Log ("ReloadScene");
			Application.LoadLevel (Application.loadedLevel);
		}
	}
}
