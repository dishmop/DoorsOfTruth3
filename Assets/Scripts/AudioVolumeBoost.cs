using UnityEngine;
using System.Collections;
using System.Linq;

public class AudioVolumeBoost : MonoBehaviour {
	public float multiplier = 3;

	
	void OnAudioFilterRead(float[] data, int channels){
	
		for (int i = 0; i < data.Count(); ++i){
			data[i] *= multiplier;
		}
		
	}
}
