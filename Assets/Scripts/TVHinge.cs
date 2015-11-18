using UnityEngine;
using System.Collections;

public class TVHinge : MonoBehaviour {

	public enum State {
		kUp,
		kGoingUp,
		kDown,
		kGoingDown,
	};
	public State state = State.kUp;
	
	
	float triggerTime;
	float period = 1.5f;
	
	public GameObject targetGO;
	Quaternion targetRot;
	Quaternion startRot;
	
	public void GoUp(){
		if (state  == State.kDown || state == State.kGoingDown){
			state = State.kGoingUp;
			triggerTime = Time.time;
		}
	}
	
	public void GoDown(){
		if (state  == State.kUp || state == State.kGoingUp){
			state = State.kGoingDown;
			triggerTime = Time.time;
		}
	}	

	// Use this for initialization
	void Start () {
		startRot = transform.localRotation;
		targetRot = targetGO.transform.localRotation;
	
	}
	
	// Update is called once per frame
	void Update () {
		// Age since last trigger
		float age = Time.time - triggerTime;
		
		// Value from 0 to 1
		float val = 0.5f - 0.5f * Mathf.Cos(age * Mathf.PI / period);
		Quaternion useRot = Quaternion.identity;
		switch (state){
			case State.kUp:{
				useRot = startRot;
				break;
			}
			case State.kDown:{
				useRot = targetRot;
				break;
			}
			case State.kGoingUp:{
				useRot = Quaternion.Slerp(targetRot, startRot, val);
				if (age > period){
					state = State.kUp;
				}
				break;
			}			
			case State.kGoingDown:{
				useRot = Quaternion.Slerp(startRot, targetRot, val);
				if (age > period){
					state = State.kDown;
				}
				break;
			}			
		}
		transform.localRotation = useRot;
	
	}
	
}
