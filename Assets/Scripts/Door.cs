using UnityEngine;
using System.Collections;
using UnityEngine.Events;
//using System.Collections.Generic;
//using UnityEngine.Analytics;

public class Door : MonoBehaviour {


	public UnityEvent openTrigger;
	public UnityEvent closeTrigger;

	public enum State{
		kClosed,
		kOpening,
		kOpen,
		kMovingToEnd,
		kClosing,
		kAjar
	}
	public float angle = 0;
	public float angularSpeed = 90;
	public bool locked = false;
	
	public State state = State.kClosed;
	public bool isInTrigger = false;
	public AudioSource openSound;
	public AudioSource closeSound;
	public GameObject finishPosGO;
	public GameObject startPosGO;
	
	Vector3 stepToNext = Vector3.zero;
	Quaternion refRot = Quaternion.identity;
	float rotTimeToTake = 1;
	float walkSpeed = 0;
	float rotTimeStart = 0;
	
	GameObject colliderGO;
	GameObject playerGO;

	bool canInteract = false;
	
	// Use this for initialization
	void Start () {
		finishPosGO.GetComponent<Renderer>().enabled = false;
		startPosGO.GetComponent<Renderer>().enabled = false;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		switch (state){	
			case State.kClosed:
				angle = 0;
				if (isInTrigger && Input.GetKeyDown(KeyCode.E) && !locked){
					state = State.kOpening;
					openSound.Play();
					closeSound.Stop();
					if (startPosGO != null){
						playerGO = colliderGO;
						playerGO.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
						Vector3 thereToStart = startPosGO.transform.position - (playerGO.transform.position - new Vector3(0, 1, 0) * playerGO.GetComponent<CharacterController>().height * 0.5f);
						walkSpeed = playerGO.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().GetWalkSpeed();
						float openTime = angularSpeed / 90;
						float distToStart = thereToStart.magnitude;
						thereToStart.Normalize();
						stepToNext =  thereToStart * Time.fixedDeltaTime * distToStart / openTime;
						openTrigger.Invoke();	
						
						string analyticsString = transform.parent.name + "_" + name;
						
						GoogleAnalytics.Client.SendTimedEventHit("gameFlow", "doorOpen", analyticsString, (Time.time - FontEnd.gameStartTime));						
						GoogleAnalytics.Client.SendScreenHit("doorOpen_" + analyticsString);					
					
//						
//						Analytics.CustomEvent("doorOpen", new Dictionary<string, object>
//						{
//							{ "name", analyticsString },
//							{ "gameTime", (Time.time - FontEnd.gameStartTime)},
//						});
											
//						Debug.Log("doorOpen - name: " + analyticsString + ", gameTime: " + (Time.time - FontEnd.gameStartTime));
					}
				}
				break;
			case State.kOpening:
				playerGO.transform.position += stepToNext;
				angle += angularSpeed * Time.fixedDeltaTime;
//				if (isInTrigger && Input.GetMouseButtonDown(0)){
//					state = State.kClosing;
//					openSound.Stop();
//					closeSound.Play();
//				}
					if (angle >= 90){
					if (finishPosGO != null){
						Vector3 thereToFinish = finishPosGO.transform.position - (playerGO.transform.position - new Vector3(0, 1, 0) * playerGO.GetComponent<CharacterController>().height * 0.5f);
						rotTimeStart = Time.fixedTime;
						rotTimeToTake = thereToFinish.magnitude / walkSpeed;
						refRot = playerGO.transform.rotation;
						thereToFinish.Normalize();
						stepToNext =  thereToFinish * Time.fixedDeltaTime * walkSpeed;
						state = State.kMovingToEnd;
					}	
					else{			
						state = State.kOpen;
					}
				}
				break;
			case State.kMovingToEnd:{
				playerGO.transform.position += stepToNext;
				playerGO.transform.rotation = Quaternion.Slerp(refRot,  finishPosGO.transform.rotation, (Time.fixedTime - rotTimeStart) / rotTimeToTake);
				Vector3 thereToFinish = finishPosGO.transform.position - (playerGO.transform.position - new Vector3(0, 1, 0) * playerGO.GetComponent<CharacterController>().height * 0.5f);
				float dist = thereToFinish.magnitude;
				if (dist < walkSpeed * Time.deltaTime){
					stepToNext = Vector3.zero;
					refRot = Quaternion.identity;
					state = State.kClosing;
					closeSound.Play();
					openSound.Stop();
					playerGO.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
					playerGO = null;
				}
				break;
			}
			case State.kOpen:
				angle = 90;
				if (finishPosGO != null){
					
				}
				else if (isInTrigger && Input.GetMouseButtonDown(0)){
					state = State.kClosing;
					closeSound.Play();
					openSound.Stop();
				}
				break;
			case State.kClosing:
				angle -= angularSpeed * Time.fixedDeltaTime;
				if (isInTrigger && Input.GetMouseButtonDown(0)){
					state = State.kOpening;
					openSound.Play();
					closeSound.Stop();
				}
				if (angle <= 0){
					state = State.kClosed;
					closeTrigger.Invoke();
				}
				break;
		}
		
		transform.FindChild("Hinge").localRotation = Quaternion.Euler(0, -angle, 0);
		
		canInteract = isInTrigger && IsLookingAt() && !locked;
		if (isInTrigger){	
			if (canInteract){	
				if ((state == State.kClosed || state == State.kClosing)){
					HUD.singleton.SetLowerTextMessage("Press [E] to open");
				}
				else{
					HUD.singleton.SetLowerTextMessage("");
				}
			}
			else{
				HUD.singleton.ClearLowerTextMessage();
			}
		}
	
	}
	
	public void Unlock(){
		locked = false;
	}
	
	public void CloseDoor(){
		state = State.kClosing;
		closeSound.Play();
	}
	
	bool IsLookingAt(){
		Vector3 fw = new Vector3(0, 0, 1);
		Vector3 playerFW = colliderGO.transform.TransformDirection(fw);
		Vector3 thereToHere = finishPosGO.transform.position - colliderGO.transform.position;
		Debug.DrawLine(colliderGO.transform.position, colliderGO.transform.position + playerFW, Color.blue);
		Debug.DrawLine(colliderGO.transform.position, colliderGO.transform.position + thereToHere, Color.red);
		thereToHere.Normalize();
		float dotResult = Vector3.Dot(playerFW, thereToHere);
		return (dotResult > 0.5f);
		
	}
	
	void OnTriggerEnter(Collider collider){
		colliderGO = collider.gameObject;
		isInTrigger = true;
	}
	
	void OnTriggerExit(Collider collider){
		colliderGO = null;
		isInTrigger = false;
		HUD.singleton.ClearLowerTextMessage();
	}
}
