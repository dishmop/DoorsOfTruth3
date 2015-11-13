using UnityEngine;
using System.Collections;

public class DeathRoom01 : MonoBehaviour {
	
	public GameObject waterGO;
	public float speed = 1;
	
	public void Trigger(){
		waterGO.SetActive(true);
	
	}
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (waterGO.activeSelf){
			waterGO.transform.position += new Vector3(0, 1, 0) * speed * Time.deltaTime;
			if (!GetComponent<AudioSource>().isPlaying){
				GetComponent<AudioSource>().Play();

			}

		}

		//Debug.Log ("Time.time - startTime = " + (Time.time - startTime));
	
	}
}
