using UnityEngine;
using UnityEngine.Events;

public class BlackBoard : MonoBehaviour {
	public bool isInTrigger = false;
	bool isInInteractionZone = false;	
	public bool canInteract = false;
	GameObject colliderGO;

	public GameObject chalkGO;
	public GameObject interactPosGO;
    
    public UnityEvent onRearrange;
    public string rearrangeFor;

	public bool isInteractable;
	public string interactionText = "Press [E] for attention";

	public bool interacting = false;

	public AudioClip[] writingSounds;
	public AudioClip[] erasingSounds;

	public void PlayWritingSound(){
		if (writingSounds.Length == 0)
			return;
		GetComponent<AudioSource> ().PlayOneShot (writingSounds [Random.Range (0, writingSounds.Length)]);
	}

	public void PlayErasingSound(){
		if (erasingSounds.Length == 0)
			return;

		GetComponent<AudioSource> ().PlayOneShot (erasingSounds [Random.Range (0, erasingSounds.Length)]);
	}
	
	bool IsLookingAt(){
		return (LookAtAngle () < 40);
	}

	void Start() {
		interactPosGO.GetComponent<Renderer> ().enabled = false;
		chalkGO.SetActive (false);
	}

	float LookAtAngle() {
		Vector3 fw = new Vector3(0, 0, 1);
		Vector3 playerFW = colliderGO.transform.TransformDirection(fw);
		Vector3 thereToHere = transform.position - colliderGO.transform.position;
		Debug.DrawLine(colliderGO.transform.position, colliderGO.transform.position + playerFW, Color.blue);
		Debug.DrawLine(colliderGO.transform.position, colliderGO.transform.position + thereToHere, Color.red);
		thereToHere.y = 0;
		thereToHere.Normalize();
		float dotResult = Vector3.Dot(playerFW, thereToHere);

		dotResult = Mathf.Clamp (dotResult, -1f, 1f);

		return Mathf.Acos (dotResult) * Mathf.Rad2Deg;
	}

	bool LookingToLeft () {
		Vector3 right = new Vector3(1, 0, 0);
		Vector3 playerRight = colliderGO.transform.TransformDirection(right);
		Vector3 thereToHere = transform.position - colliderGO.transform.position;
		thereToHere.y = 0;
		float dotResult = Vector3.Dot(playerRight, thereToHere);

		return dotResult > 0f;
	}


	void FixedUpdate () {
		if (interacting) {
			interactionText = "Press [E] to leave";

			// reposition to look at and stand in front of board
			float walkSpeed = colliderGO.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().GetWalkSpeed();
			float rotationSpeed = 90;
			Vector3 PositionOffset = colliderGO.transform.position - interactPosGO.transform.position;
			PositionOffset.y = 0;

			if (PositionOffset.magnitude > walkSpeed * Time.fixedDeltaTime) {
				colliderGO.transform.position -= PositionOffset.normalized *walkSpeed* Time.fixedDeltaTime;
			} else {
				colliderGO.transform.position -= PositionOffset;
			}

			if (LookAtAngle() > rotationSpeed*Time.fixedDeltaTime){
				colliderGO.transform.Rotate(new Vector3(0,rotationSpeed*Time.fixedDeltaTime*(LookingToLeft()?1f:-1f),0));
			} else {
				Vector3 level = transform.position;
				level.y = colliderGO.transform.position.y;
				colliderGO.transform.LookAt(level);
			}
            
            if(rearrangeFor!=null){
                if(GetComponentInChildren<Equations>().ArrangedFor == rearrangeFor) {
                    onRearrange.Invoke();
                    isInteractable = false;
                    interacting = false;
                    
                    Cursor.lockState = interacting?CursorLockMode.None:CursorLockMode.Locked;

					var controller = FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
					controller.lockRotation = interacting;
					controller.lockPosition = interacting;

					chalkGO.SetActive(interacting);
                }
            }

		} else {
			interactionText = "Press [E] to use";
		}

		if (isInTrigger){
			canInteract = IsLookingAt();
			if (canInteract && isInteractable){
				if(!isInInteractionZone){
					//onEnterInteractionZone.Invoke();
				}
				isInInteractionZone = true;
				HUD.singleton.SetLowerTextMessage(interactionText);
				if (Input.GetKeyDown(KeyCode.E)){
					//onInteract.Invoke();
					//string analyticsString = transform.parent.name + "_" + name;
					//GoogleAnalytics.Client.SendTimedEventHit("gameAction", "tvAttention", analyticsString, (Time.time - FontEnd.gameStartTime));
					//										
					//					Analytics.CustomEvent("tvAttention", new Dictionary<string, object>
					//					                      {
					//						{ "name", analyticsString },
					//						{ "gameTime", (Time.time - FontEnd.gameStartTime)},
					//					});
					
					//Debug.Log("tvAttention - name: " + analyticsString + ", gameTime: " + (Time.time - FontEnd.gameStartTime));
					
					//Transform ding = transform.FindChild("Ding");
					//if (ding != null){
					//	ding.GetComponent<AudioSource>().Play ();
					//}

					var controller = FindObjectOfType<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
					interacting = !interacting;
					//Cursor.visible = interacting;

					Cursor.lockState = interacting?CursorLockMode.None:CursorLockMode.Locked;

					controller.lockRotation = interacting;
					controller.lockPosition = interacting;

					chalkGO.SetActive(interacting);
				}
			}
			else{
				if(isInInteractionZone){
					//onExitInteractionZone.Invoke();
					HUD.singleton.ClearLowerTextMessage();
				}
				isInInteractionZone = false;
				
			}
		}
		else{
			if(isInInteractionZone){
				//onExitInteractionZone.Invoke();
			}
			isInInteractionZone = false;
		}

	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.GetComponentInChildren<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> () != null) {
			colliderGO = collider.gameObject;
			isInTrigger = true;
		}
	}
	
	void OnTriggerExit(Collider collider){
		if (collider.gameObject.GetComponentInChildren<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> () != null) {
			colliderGO = null;
			isInTrigger = false;
			HUD.singleton.ClearLowerTextMessage ();
		}
	}

}
