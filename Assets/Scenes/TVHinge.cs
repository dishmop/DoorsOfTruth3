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
	
	float theta = 0;
	float triggerTime;
	float period = 1.5f;
	
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
	
	}
	
	// Update is called once per frame
	void Update () {
		// Age since last trigger
		float age = Time.time - triggerTime;
		
		// Value from 0 to 1
		float val = 0.5f - 0.5f * Mathf.Cos(age * Mathf.PI / period);
		
		switch (state){
			case State.kUp:{
				theta = 0;
				break;
			}
			case State.kDown:{
				theta = -90;
				break;
			}
			case State.kGoingUp:{
				theta = Mathf.Lerp(-90, 0, val);
				if (age > period){
					state = State.kUp;
				}
				break;
			}			
			case State.kGoingDown:{
				theta = Mathf.Lerp(0, -90, val);
				if (age > period){
					state = State.kDown;
				}
				break;
			}			
		}
		
		Vector3 rot = transform.localRotation.eulerAngles;
		rot.x = theta;
		transform.localRotation = Quaternion.Euler(rot);
		
		
	
	}
}
