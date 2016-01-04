using UnityEngine;
using System.Collections;

public class Reciprocal : MonoBehaviour {
	public Expression expression;

	void Start(){
		GetComponent<UnityEngine.UI.RawImage> ().color = Equations.DefaultTextColour;

	}


	void Update() {
		if (expression.IsDraggingUnder) {
			GetComponent<UnityEngine.UI.RawImage> ().enabled = true;

			Rect box = expression.ScreenRect;
			box.yMax += 3f;
			box.yMin = box.yMax;
			box.yMax += 5f;

			if (expression.ReciprocalSide) {
				if (box.xMin < 0) {
					box.xMin = 0;
				}
			} else {
				if (box.xMax > 0) {
					box.xMax = 0;
				}
			}

			transform.localPosition = new Vector3 (box.x, box.y, 0);
			((RectTransform)transform).sizeDelta = new Vector3 (box.size.x, box.size.y, 0);
		}else if(expression.GetType() == typeof(Product)) {
			Product prod = (Product)expression;

			if(prod.NumBottom>0){
				GetComponent<UnityEngine.UI.RawImage> ().enabled = true;

				Rect box = prod.ScreenRect;
				box.yMax += 3f;

				box.yMin = box.yMax;
				box.yMax += 5f;
	
				box.y -= prod.HeightTop;
	
				if(prod.ShowingSign){
					box.xMin+=35f;
				}

				transform.localPosition = new Vector3 (box.x, box.y, 0);
				((RectTransform)transform).sizeDelta = new Vector3 (box.size.x, box.size.y, 0);

			} else {
				GetComponent<UnityEngine.UI.RawImage> ().enabled = false;

			}

		} else {
			GetComponent<UnityEngine.UI.RawImage> ().enabled = false;
		}

		transform.localRotation = new Quaternion ();
	}
}
