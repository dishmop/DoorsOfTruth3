using UnityEngine;
using System.Collections;

public class AsyncLevelLoader : MonoBehaviour {

	public string levelName;

	// Use this for initialization
	IEnumerator Start() {
		AsyncOperation async = Application.LoadLevelAdditiveAsync(levelName);
		yield return async;
		Debug.Log("Loading complete");
	}
	

	
	// Update is called once per frame
	void Update () {
	
	}
}
