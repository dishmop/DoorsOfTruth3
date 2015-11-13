using UnityEngine;
using System.Collections;

public class DeadRoomWheel : MonoBehaviour {
	public GameObject tv;
	public GameObject cylinder;
	public float speed = 0.01f;
	float triggerTime = 0;
	float triggerDelay = 0;
	
	bool triggered;
	
	public void Trigger(float delay){
		triggered = true;
		triggerTime = Time.time;
		triggerDelay = delay;
		
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (triggered && Time.time > triggerTime + triggerDelay){
			if (!GetComponent<ASDAudioSource>().IsPlaying()){
				GetComponent<ASDAudioSource>().Play();
			}
			Vector3 pos = cylinder.transform.position;
			float distMoved = speed * Time.deltaTime;
			pos.x += distMoved;
			cylinder.transform.position = pos;		
			
			float circum = Mathf.PI * cylinder.transform.localScale.x;
			float degrees = 360 * distMoved / circum;
			cylinder.transform.Rotate(0, degrees, 0);
		}
	
	}
}
