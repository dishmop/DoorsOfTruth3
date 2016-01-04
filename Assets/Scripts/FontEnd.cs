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
	public string levelName1;
	public string levelName2;
	public string levelName3;
	string useLevelName;
		
	public void StartGame(){
		triggerStartGame = true;
		startTime = Time.time;
		GetComponent<AudioSource>().Play ();
		useLevelName = levelName1;
		SpawnManager.ClearSpawnPos();
	}
	
	public void StartLevel2(){
		triggerStartGame = true;
		startTime = Time.time;
		GetComponent<AudioSource>().Play ();
		useLevelName = levelName2;
		SpawnManager.ClearSpawnPos();
	}

	public void StartLevel3(){
		triggerStartGame = true;
		startTime = Time.time;
		GetComponent<AudioSource>().Play ();
		useLevelName = levelName3;
		SpawnManager.ClearSpawnPos();
	}

	
	// Update is called once per frame
	void Update () {
	
		if (triggerStartGame){
			
			fade = Mathf.Lerp (0, 1, (Time.time - startTime) / duration);
			transform.FindChild("Cover").GetComponent<Image>().color = new Color (0, 0, 0, fade);
			
		}
		if (fade > 0.99f){
			Application.LoadLevel(useLevelName);
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
