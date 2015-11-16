using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject spawnPoint	;

	// Use this for initialization
	void Start () {
		GameObject useSpawnPoint = null;
		GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = false;
		if (SpawnManager.nextSpawnPoint != null && SpawnManager.nextSpawnPoint != ""){
			useSpawnPoint = GameObject.Find(SpawnManager.nextSpawnPoint);
		}
		if (useSpawnPoint == null && SpawnManager.nextSpawnPos != Vector3.zero){
			transform.position = SpawnManager.nextSpawnPos;
			transform.rotation = SpawnManager.nextSpawnRot;
			SpawnManager.ClearSpawnPos();
			GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
			return;
			
		}
		if (useSpawnPoint == null && spawnPoint != null){
			useSpawnPoint = spawnPoint;
		}
		if (useSpawnPoint != null){
			Vector3 pos = useSpawnPoint.transform.position;
			pos.y += 0.2f;
			transform.position = pos;
			transform.rotation = useSpawnPoint.transform.rotation;
		}
		GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().enabled = true;
		
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
