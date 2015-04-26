using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject enemiePrefab;

	[Range(1f, 100f)]
	public float rate;

	[Range(5f, 20f)]
	public float minRange;

	public AudioAnalyser analyser;

	float time;

	// Use this for initialization
	void Start () {
		time = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		time += analyser.sValueL + analyser.sValueR;
		if (time >= rate) {
			time = 0f;
			GameObject newEnemie = Instantiate (enemiePrefab) as GameObject;
			Vector3 newPosition = minRange * Random.onUnitSphere;
			newPosition.z = 0f;
			newEnemie.transform.position = newPosition;
		}
	}
}
