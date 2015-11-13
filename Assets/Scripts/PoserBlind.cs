using UnityEngine;
using System.Collections;
using System.Linq;

public class PoserBlind : MonoBehaviour {

	bool isOpen = true;
	bool isInTrigger;
	bool canInteract;
	bool isInteractable = true;
	GameObject colliderGO;
	float dist = 0;
	float speed = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	string GetInteractionText(){
		if (isOpen){
			return "Press E to pull down the blind";
		}
		else{
			return "Press E to raise the blind";
		}
	}
	
	void Trigger(){
		Debug.Log("trigger");
		isOpen = !isOpen;
		GetComponent<AudioSource>().Play();
	}
	
	// Update is called once per frame
	void Update () {
//		transform.FindChild("Blind").gameObject.SetActive(!isOpen);
		if (isInTrigger){
			canInteract = IsLookingAt();
			if (canInteract && isInteractable){
				HUD.singleton.SetLowerTextMessage(GetInteractionText());
				if (Input.GetKeyDown(KeyCode.E)){
					Trigger();
					string analyticsString = transform.parent.name + "_" + name;
					GoogleAnalytics.Client.SendTimedEventHit("gameAction", "tvAttention", analyticsString, (Time.time - FontEnd.gameStartTime));
					//										
					//					Analytics.CustomEvent("tvAttention", new Dictionary<string, object>
					//					                      {
					//						{ "name", analyticsString },
					//						{ "gameTime", (Time.time - FontEnd.gameStartTime)},
					//					});
					
					//Debug.Log("tvAttention - name: " + analyticsString + ", gameTime: " + (Time.time - FontEnd.gameStartTime));
					
					Transform ding = transform.FindChild("Ding");
					if (ding != null){
						ding.GetComponent<AudioSource>().Play ();
					}
				}
			}
			else{
				HUD.singleton.ClearLowerTextMessage();
			}
		}	
		if (isOpen){
			dist -= speed * Time.deltaTime;
		}
		else{
			dist += speed * Time.deltaTime;
		}
		dist = Mathf.Clamp01(dist);
		Vector3 scale = transform.FindChild("Blind").localScale;
		scale.y = dist * 0.75f;
		transform.FindChild("Blind").localScale = scale;
		
		Vector3 pos = transform.FindChild("Blind").localPosition;
		pos.y = 0.5f - 0.5f * dist * 0.75f;
		transform.FindChild("Blind").localPosition = pos;
		
		Vector2[] uvs = transform.FindChild("Blind").GetComponent<MeshFilter>().mesh.uv;
		uvs[1].y = dist;
		uvs[3].y = dist;
		
		transform.FindChild("Blind").GetComponent<MeshFilter>().mesh.uv = uvs;
		transform.FindChild("Blind").GetComponent<MeshFilter>().mesh.UploadMeshData(false);
	}
	
	
	bool IsLookingAt(){
		Vector3 fw = new Vector3(0, 0, 1);
		Vector3 playerFW = colliderGO.transform.TransformDirection(fw);
		Vector3 thereToHere = transform.position - colliderGO.transform.position;
		Debug.DrawLine(colliderGO.transform.position, colliderGO.transform.position + playerFW, Color.blue);
		Debug.DrawLine(colliderGO.transform.position, colliderGO.transform.position + thereToHere, Color.red);
		thereToHere.Normalize();
		float dotResult = Vector3.Dot(playerFW, thereToHere);
		return (dotResult > 0.5f);
		
	}
	
	void OnTriggerEnter(Collider collider){
		colliderGO = collider.gameObject;
		isInTrigger = true;
	}
	
	void OnTriggerExit(Collider collider){
		colliderGO = null;
		isInTrigger = false;
		HUD.singleton.ClearLowerTextMessage();
	}
}
