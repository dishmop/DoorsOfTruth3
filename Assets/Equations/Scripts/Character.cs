using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	public string character = "";
	public Color color = Color.black;

	void Awake() {
		color = Equations.DefaultTextColour;
	}

	public void LateUpdate() {

		GetComponent<UnityEngine.UI.Text> ().text = character;
		GetComponent<UnityEngine.UI.Text> ().color = color;

		transform.localRotation = new Quaternion ();
	}
}
