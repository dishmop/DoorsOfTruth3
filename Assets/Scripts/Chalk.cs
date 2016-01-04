using UnityEngine;
using System.Collections;

public class Chalk : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// set chalk to mouse position
		Vector3 worldPos;
		RectTransformUtility.ScreenPointToWorldPointInRectangle ((RectTransform)transform.parent.GetComponentInChildren<Equations>().transform, Input.mousePosition, Camera.main, out worldPos);
		transform.position = worldPos;
		transform.position = transform.position + FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().transform.TransformDirection(new Vector3 (0.02f,-0.02f,-0.1f));
	}
}
