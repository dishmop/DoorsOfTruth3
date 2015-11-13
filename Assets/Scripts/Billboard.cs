using UnityEngine;
using System.Collections;



public class Billboard : MonoBehaviour
{
	
	void Update()
	{
		Camera m_Camera = Camera.main;
		
		transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.back,
		                 m_Camera.transform.rotation * Vector3.up);
	}
}