using UnityEngine;
using System.Collections;

public class DeadRoom03English : MonoBehaviour {
	public GameObject penGrid;
	public GameObject ceiling;
	public float ceilingSpeed = 1;
	public float penSpeed = 1;
	
	
	float triggerTime = 0;
	float triggerDelay = 0;
	bool triggered;
	
	public void Trigger(float delay){
		triggerDelay = delay;
		triggerTime = Time.time;	
		triggered = true;	
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!triggered) return;
		
		if (Time.time > triggerTime + triggerDelay){
			if (!GetComponent<ASDAudioSource>().IsPlaying()){
				GetComponent<ASDAudioSource>().Play();
			}
			if (penGrid.transform.localPosition.z > 0){
				Vector3 localPos = penGrid.transform.localPosition;
				localPos.z -= penSpeed * Time.deltaTime;
				penGrid.transform.localPosition = localPos;
			}
			if (ceiling.transform.localPosition.y > -2.3){
				Vector3 localPos = ceiling.transform.localPosition;
				localPos.y -= ceilingSpeed * Time.deltaTime;
				ceiling.transform.localPosition = localPos;
			}
		}
	
	}
}
