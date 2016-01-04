using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	public string character = "";
	Color color = Color.cyan;
	public Equations equations;

	public void SetColour (Color colour) {
		color = colour;
	}

	void Start() {
		if (color == Color.cyan) {
			color = equations.defaultTextColour;
		}
	}

	public void LateUpdate() {

		GetComponent<UnityEngine.UI.Text> ().text = character;
		GetComponent<UnityEngine.UI.Text> ().color = color;

		transform.localRotation = new Quaternion ();
	}
}
