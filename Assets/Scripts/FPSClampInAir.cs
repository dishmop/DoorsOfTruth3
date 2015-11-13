using UnityEngine;
using System.Collections;

public class FPSClampInAir : MonoBehaviour {

	float walkSpeed = 0;
	float runSpeed = 0;
	
	// Use this for initialization
	void Start () {
		walkSpeed = GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().GetWalkSpeed();
		runSpeed = GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().GetRunSpeed();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().IsGrounded()){
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().SetWalkSpeed(walkSpeed);
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().SetRunSpeed(runSpeed);
		}
		else{
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().SetWalkSpeed(0);
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().SetRunSpeed(0);
		}
	
	}
}
