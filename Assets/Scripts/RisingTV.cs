using UnityEngine;
using System.Collections;

public class RisingTV : MonoBehaviour {
	public float speed = 1f;
	
	bool hasTriggered;

	public void Trigger(){
//		Debug.Log ("Trigger rise");
		hasTriggered = true;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (hasTriggered){
			Vector3 pos = transform.position;
			pos.y += speed * Time.deltaTime;
			transform.position = pos;
		}
	
	
	}
}
