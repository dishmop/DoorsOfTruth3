using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

	public static string nextSpawnPoint;
	public static Vector3 nextSpawnPos = Vector3.zero;
	public static Quaternion nextSpawnRot = Quaternion.identity;
	
	public static void ClearSpawnPos(){
		nextSpawnPos = Vector3.zero;
		nextSpawnRot = Quaternion.identity;
		
	}

	public void RecordPlayerPos(){
		nextSpawnPos = Camera.main.transform.position;
		nextSpawnRot = Camera.main.transform.rotation;
		nextSpawnPoint = "";
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
