using UnityEngine;
using System.Collections;

public class Wheel : MonoBehaviour {
	 float angularSpeed = 36;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(angularSpeed * Time.deltaTime, 0, 0);
	
	}
}
