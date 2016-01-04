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
		if (mouseOver) {
			GetComponent<Rigidbody> ().isKinematic = true;

			transform.position = Vector3.Lerp(transform.position, startpos, 0.6f);

			GetComponent<BoxCollider>().size = new Vector3(1,5,1);

		} else {
			GetComponent<Rigidbody> ().isKinematic = false;
			GetComponent<BoxCollider>().size = new Vector3(1,1,1);

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
