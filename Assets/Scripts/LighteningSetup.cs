using UnityEngine;
using System.Collections;

public class LighteningSetup : MonoBehaviour {
	public GameObject obj1;
	public GameObject obj2;

	// Use this for initialization
	void Start () {
		Update ();

	
	}
	
	// Update is called once per frame
	void Update () {
		Lightening lightening = GetComponent<Lightening>();
		Vector3 fromHereToThere = obj1.transform.position - obj2.transform.position;
		fromHereToThere.Normalize();
		
		lightening.startPoint = obj1.transform.position - fromHereToThere * 0.5f * obj2.transform.localScale.x;
		lightening.endPoint = obj2.transform.position + fromHereToThere * 0.5f * obj2.transform.localScale.x;
		
		float len = (lightening.startPoint  - lightening.endPoint).magnitude;
		lightening.numStages = Mathf.Max ((int)(len * 10), 2);
		lightening.size =   1f;
		lightening.ConstructMesh();
	
	}
}
