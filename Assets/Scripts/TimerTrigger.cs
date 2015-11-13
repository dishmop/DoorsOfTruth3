using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TimerTrigger : MonoBehaviour {
	public UnityEvent triggerFunc;
	float startTime = 0;
	float timerDelay = 0;
	bool hasStarted = false;

	public void StartTimer(float delay){
		startTime = Time.time;
		timerDelay = delay;
		hasStarted = true;
	
	}

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().enabled = false;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (hasStarted){
		//	Debug.Log(name + ": Time elapsed = " + (Time.time - startTime));
		}
		if (hasStarted && Time.time > startTime + timerDelay){
			triggerFunc.Invoke();
		}
	
	}
}
