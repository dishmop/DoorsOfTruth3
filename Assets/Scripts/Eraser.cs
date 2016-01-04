using UnityEngine;
using System.Collections;

public class Eraser : MonoBehaviour {

	Vector3 startpos;

	public Equations equations;

	// Use this for initialization
	void Start () {
		startpos = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (mouseOver && transform.position.y <= startpos.y) {
			GetComponent<Rigidbody> ().AddForce (new Vector3 (0, 0.1f, 0), ForceMode.VelocityChange);
		}

		if (mouseOver) {
			GetComponent<Rigidbody> ().AddForce (-Physics.gravity, ForceMode.Acceleration);

		}
	}

	void Update() {
		if(mouseOver && Input.GetMouseButtonDown(0)) {
			equations.Undo();
		}

	}


	public bool mouseOver = false;
	bool mouseOverOld = false;

	void OnMouseOver() {
		// when mouse is over board rubber
		mouseOver = true;
	}

	void OnMouseExit(){
		mouseOver = false;
	}
}
