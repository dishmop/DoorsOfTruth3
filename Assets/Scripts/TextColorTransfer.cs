using UnityEngine;
using System.Collections;

public class TextColorTransfer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Renderer>().material.color = GetComponent<TextMesh>().color;
	
	}
}
