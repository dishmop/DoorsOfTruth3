using UnityEngine;
using System.Collections;

public class HatBat : MonoBehaviour {

	public bool isOn = false;
	public  float freq = 1;
	public  float mag = 30f;
	float startTime = -1f;
	float startAngle;
	GameObject hingeGO;

	// Use this for initialization
	void Start () {
		hingeGO = transform.FindChild("Rod").FindChild("Hinge").gameObject;
		startAngle = hingeGO.transform.localRotation.y;
	
	}
	
	// Update is called once per frame
	void Update () {
		// If just turning on
		if (startTime < 0 && isOn){
			startTime = Time.time;
		}
		if (isOn){
			float age = Time.time - startTime;
			float angleDelta = mag * Mathf.Sin(freq * age / 2f * Mathf.PI);
			Vector3 rot = hingeGO.transform.localRotation.eulerAngles;
			rot.x = startAngle + angleDelta;
			hingeGO.transform.localRotation = Quaternion.Euler(rot);
		}
		
	
	}
}
