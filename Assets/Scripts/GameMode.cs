using UnityEngine;
using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.Analytics;

public class GameMode : MonoBehaviour {

	public static GameMode singleton = null;
	public GameObject deadPanelGO;
	public string nextLevelName;
	public string reloadLevelName = "Playpen";
	
	public enum State{
		kStartingLevel,
		kPlaying,
		kFadeout,
		kShutEyes,
		kEyesShut,
		kOpenEyes,
		kDead,
		kUI,
	}
	float fadeStart = 0;
	float fadeDuration = 4;
	float shutEyeFrac = 0;
	
	public State state = State.kPlaying;
	
	public void ShutEyes(){
		state = State.kShutEyes;
		shutEyeFrac = 0;
	}

	public void OpenEyes(){
		state = State.kOpenEyes;
		shutEyeFrac = 1;
	}
		// Use this for initialization
	void Start () {
		if (state != State.kUI){
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		
		if (Application.loadedLevelName == "GameComplete"){
			//Debug.Log("gameComplete - gameTime: " + (Time.time - FontEnd.gameStartTime));
			GoogleAnalytics.Client.SendTimedEventHit("gameFlow", "gameComplete", "", (Time.time - FontEnd.gameStartTime));
			GoogleAnalytics.Client.SendScreenHit("gameComplete");
//			Analytics.CustomEvent("gameComplete", new Dictionary<string, object>
//			{
//				{ "gameTime", (Time.time - FontEnd.gameStartTime)},
//			});			
		}
	
	}
	
	public void StaticTriggerDead(string objName){
		if (singleton.state != State.kPlaying) return;
		
		SpawnManager.nextSpawnPoint = objName;
		singleton.state = State.kFadeout;
		singleton.fadeStart = Time.time;
	}
	
	
	public void TriggerDead(GameObject spawnPoint){
		if (state != State.kPlaying) return;
		SpawnManager.nextSpawnPoint = spawnPoint.name;
		state = State.kFadeout;
		fadeStart = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == State.kUI){
			return;
		}
		if (state == State.kEyesShut){
			Camera.main.gameObject.transform.FindChild("EyeLids").gameObject.SetActive(true);
			Camera.main.gameObject.transform.FindChild("EyeLids").GetComponent<Renderer>().material.color = new Color(0, 0, 0, 1);
			return ;
		}
		if (state == State.kShutEyes){
//			Debug.Log("Shut eyes");
			Camera.main.gameObject.transform.FindChild("EyeLids").gameObject.SetActive(true);
			shutEyeFrac += 1 * Time.deltaTime;
			Camera.main.gameObject.transform.FindChild("EyeLids").GetComponent<Renderer>().material.color = new Color(0, 0, 0, shutEyeFrac);
			if (shutEyeFrac > 1){
				Application.LoadLevel (nextLevelName);
			}
				
		}
		if (state == State.kOpenEyes){
			Camera.main.gameObject.transform.FindChild("EyeLids").gameObject.SetActive(true);
			shutEyeFrac -= 1 * Time.deltaTime;
			Camera.main.gameObject.transform.FindChild("EyeLids").GetComponent<Renderer>().material.color = new Color(0, 0, 0, shutEyeFrac);
			if (shutEyeFrac < 0){
				Camera.main.gameObject.transform.FindChild("EyeLids").gameObject.SetActive(false);
			}
			
		}		
		if (state == State.kPlaying){
			deadPanelGO.SetActive(false);
		}
		if (state == State.kFadeout){
			deadPanelGO.SetActive(true);
			float alpha = Mathf.Lerp (0, 1, (Time.time - fadeStart) / fadeDuration);
			deadPanelGO.GetComponent<DeadPanel>().alpha =alpha;
			if (alpha > 0.99f){
				state  = State.kDead;
			}
		}
		if (state == State.kDead){
			if (Input.GetKeyDown(KeyCode.E)){
				Application.LoadLevel(reloadLevelName);
				state = State.kPlaying;
			}
		}
		
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
