using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeadPanel : MonoBehaviour {
	public float alpha = 1;

	// Use this for initialization
	void Start () {
		Update ();
	
	}
	
	// Update is called once per frame
	void Update () {
		Color col = GetComponent<Image>().color;
		col.a = alpha;
		
		GetComponent<Image>().color = col;
		
		col = transform.FindChild("DeadPanel").GetComponent<Image>().color;
		col.a = alpha;
		transform.FindChild("DeadPanel").GetComponent<Image>().color = col;
		
		col = transform.FindChild("DeadPanel").FindChild("Dead Message").GetComponent<Text>().color;
		col.a = alpha;
		transform.FindChild("DeadPanel").FindChild("Dead Message").GetComponent<Text>().color = col;
		
		
		
	
	}
}
