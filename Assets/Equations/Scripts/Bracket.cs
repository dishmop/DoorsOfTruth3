using UnityEngine;
using System.Collections;

public class Bracket : MonoBehaviour {

	// end ticks
	GameObject left;
	GameObject right;

	public Equations equations;

	public Expression expression;
	public bool Top; // whether too bracket or bottom bracket

	void Start(){
		left = (GameObject)Instantiate (equations.bracketEnd);
		left.transform.SetParent(equations.transform,false);
		right = (GameObject)Instantiate (equations.bracketEnd);
		right.transform.SetParent (equations.transform,false);

		transform.SetParent(equations.transform,false);
		transform.localRotation = new Quaternion ();
		left.transform.localRotation = new Quaternion ();
		right.transform.localRotation = new Quaternion ();

		left.GetComponent<UnityEngine.UI.RawImage> ().color = equations.defaultTextColour;
		right.GetComponent<UnityEngine.UI.RawImage> ().color = equations.defaultTextColour;
		GetComponent<UnityEngine.UI.RawImage> ().color = equations.defaultTextColour;

	}

	void Update() {
		if (expression == null) {
			Destroy (gameObject);
		} else {
			if (Top) { // bracket above (for multiplication and division)
				GetComponent<UnityEngine.UI.RawImage> ().enabled = true;
			
				Rect box = expression.ScreenRect;
				box.yMin = box.yMax;
				box.yMax += 3f;

				transform.localPosition = new Vector3 (box.x, box.y, 0);
				((RectTransform)transform).sizeDelta = new Vector3 (box.size.x, box.size.y, 0);

				left.transform.localPosition = new Vector3 (box.xMin + 1.5f, box.yMin - 5, 0);
				right.transform.localPosition = new Vector3 (box.xMax - 1.5f, box.yMin - 5, 0);
			} else { // bracket below (for addition and subtraction)
				GetComponent<UnityEngine.UI.RawImage> ().enabled = true;
			
				Rect box = expression.ScreenRect;
				box.yMin += 5f;
				box.yMax = box.yMin;
				box.yMin -= 3f;

				transform.localPosition = new Vector3 (box.x, box.y, 0);
				((RectTransform)transform).sizeDelta = new Vector3 (box.size.x, box.size.y, 0);

				left.transform.localPosition = new Vector3 (box.xMin + 1.5f, box.yMin + 5, 0);
				right.transform.localPosition = new Vector3 (box.xMax - 1.5f, box.yMin + 5, 0);
			}
		}
	}

	void OnEnable() {
		if (left != null && right != null) {
			left.SetActive(true);
			right.SetActive(true);
		}
	}

	void OnDisable() {
		if (left != null && right != null) {
			left.SetActive(false);
			right.SetActive(false);
		}
	}

	void OnDestroy() {
		Destroy (left);
		Destroy (right);
	}

	public void BringToFront() {
		transform.SetAsLastSibling ();
		if (left != null && right != null) {
			left.transform.SetAsLastSibling ();
			right.transform.SetAsLastSibling ();
		}
	}
}
