using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using System.Collections.Generic;
//using UnityEngine.Analytics;

public class FontEnd : MonoBehaviour {
	public static float gameStartTime = 0;
	
	float fade = 0;
	bool triggerStartGame = false;
	float duration = 1;
	float startTime = 0;
	public string levelName;
	
	public void StartGame(){
		triggerStartGame = true;
		startTime = Time.time;
		GetComponent<AudioSource>().Play ();
	}

	
	// Update is called once per frame
	void Update () {
	
		if (triggerStartGame){
			
			fade = Mathf.Lerp (0, 1, (Time.time - startTime) / duration);
			transform.FindChild("Cover").GetComponent<Image>().color = new Color (0, 0, 0, fade);
			
		}
		if (fade > 0.99f){
			Application.LoadLevel(levelName);
			gameStartTime = Time.time;
			//Debug.Log("startGame");
			GoogleAnalytics.Client.SendEventHit("gameFlow", "startGame");	
			GoogleAnalytics.Client.SendScreenHit("startGame");					
//			Analytics.CustomEvent("startGame", new Dictionary<string, object>
//			{
//				{ "dummy", 0},
//			});			
		}
		
	
	}
}
