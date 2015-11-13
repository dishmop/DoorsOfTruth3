using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	public static HUD singleton = null;
	
	
	public void SetLowerTextMessage(string text){
		transform.FindChild("LowerText").GetComponent<Text>().text = text;
	
	}
	
	public void ClearLowerTextMessage(){
		transform.FindChild("LowerText").GetComponent<Text>().text = "";
		
	}
	
	// Use this for initialization
	void Start () {
		ClearLowerTextMessage();
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Awake(){
		// Singleton
		if (singleton != null) Debug.LogError ("Error assigning singleton");
		singleton = this;
	}
	
	void OnDestroy(){
		singleton = null;
	}
}
