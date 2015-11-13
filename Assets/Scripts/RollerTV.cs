using UnityEngine;
using System.Collections;

public class RollerTV : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.name == "Roller"){
			GetComponent<Rigidbody>().isKinematic = false;
		}
	}
}
