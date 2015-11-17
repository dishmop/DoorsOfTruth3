using UnityEngine;
using System.Collections;
using UnityEngine.Events;
//using System.Collections.Generic;
//using UnityEngine.Analytics;

public class VideoPlay : MonoBehaviour {

	public MovieTexture[] videos;
	public GameObject pictureGO;
	public bool isInTrigger;
	public GameObject colliderGO;
	public UnityEvent onVideoFinish;
	public UnityEvent onInteract;
	public UnityEvent onEnterInteractionZone;
	public UnityEvent onExitInteractionZone;
	public GameObject slaveTVGO;
	public bool isInteractable;
	public string interactionText = "Press [E] for attention";
	public GameObject smashAudioGO;
	bool smashDone;
	bool canInteract = false;
	bool isPlaying = false;
	bool isInInteractionZone = false;

	
	

	
	// Update is called once per frame
	void Update () {
		if (isInTrigger){
			canInteract = IsLookingAt();
			if (canInteract && isInteractable){
				if(!isInInteractionZone){
					onEnterInteractionZone.Invoke();
				}
				isInInteractionZone = true;
				HUD.singleton.SetLowerTextMessage(interactionText);
				if (Input.GetKeyDown(KeyCode.E)){
					onInteract.Invoke();
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
				if(isInInteractionZone){
					onExitInteractionZone.Invoke();
				}
				isInInteractionZone = false;
				
				HUD.singleton.ClearLowerTextMessage();
			}
		}
		else{
			if(isInInteractionZone){
				onExitInteractionZone.Invoke();
			}
			isInInteractionZone = false;
		}
		
		if (isPlaying){
			MovieTexture emissiveVideo = pictureGO.transform.GetComponent<Renderer>().material.GetTexture("_EmissionMap") as MovieTexture;
			if (!emissiveVideo.isPlaying){
				onVideoFinish.Invoke();
			}
		}
		
		TestSmash();
		
		
		
	}
	
	void TestSmash(){
		if (!smashDone && MathUtils.FP.Feq(transform.rotation.eulerAngles.x, 270, 1)){
			smashAudioGO.GetComponent<AudioSource>().Play();
			smashDone = true;
		}
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
	
	void OnEnter(){
		smashDone = false;
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
	
	// Can't be called from an event
	public void PlayVideo(int index, bool loop){
	
	   StopPlayback();

		pictureGO.transform.GetComponent<Renderer>().material.SetTexture("_EmissionMap", videos[index]);
		MovieTexture emissiveVideo = pictureGO.transform.GetComponent<Renderer>().material.GetTexture("_EmissionMap") as MovieTexture;
		
		emissiveVideo.Play();
		emissiveVideo.loop = loop;
		
		GetComponent<AudioSource>().clip = videos[index].audioClip;
		GetComponent<AudioSource>().Play();
		
		isPlaying = true;
	//	Debug.Log ("PlayVideo");
	
		if (slaveTVGO != null){
			slaveTVGO.GetComponent<VideoPlay>().PlayVideo(index, loop);			    
		}
		    
		
	}
	
	public void EnableInteraction(bool enable){
//		Debug.Log ("EnableInteraction: " + enable);
		
		isInteractable = enable;
	}
	
	public void PlayVideo(int index){
		PlayVideo(index, false);
	}
	
	public void PlayLoopedVideo(int index){
		PlayVideo(index, true);
	}
	
	public void StopPlayback(){
		MovieTexture oldVideo = pictureGO.transform.GetComponent<Renderer>().material.GetTexture("_EmissionMap") as MovieTexture;
		if (oldVideo != null && oldVideo.isPlaying){
			oldVideo.Stop();
		}
		AudioSource oldSource = GetComponent<AudioSource>();
		if (oldSource != null && oldSource.isPlaying){
			oldSource.Stop ();
		}
		isPlaying = false;
		if (slaveTVGO != null){
			slaveTVGO.GetComponent<VideoPlay>().StopPlayback();			    
		}
		
	}
}
