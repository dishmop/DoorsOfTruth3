using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PositionTrigger : MonoBehaviour {

	public UnityEvent triggerHandler;

	// Use this for initialization
	void Start () {
	
		GetComponent<Renderer>().enabled = false;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider collider){
		if (collider.name == "FPSController"){
			triggerHandler.Invoke();
		}
		
	}
}
