using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour {

	public Transform handle;
	public Transform min, max;

	[HideInInspector]
	public AudioSource mAudio;

	float lastTime, offsetx;
	Vector3 position;

	void Awake () {
		mAudio = GetComponent<AudioSource>();
	}

	void Start () {
		mAudio.time = PlayerPrefs.GetFloat ("last_audio_time", 0f);
		mAudio.volume = 0f;
		mAudio.Play();

		offsetx = (max.position.x - min.position.x) / mAudio.clip.length;
		position = new Vector3 ();
		position.x = min.position.x + mAudio.time * offsetx;
		position.y = min.position.y;

		handle.position = position;
	}
	
	// Update is called once per frame
	void Update () {
		position.x = min.position.x + mAudio.time * offsetx;
		handle.position = position;
		lastTime = mAudio.time;
		if (mAudio.volume < 1f) mAudio.volume += 0.01f;
	}

	void OnDestroy () {
		PlayerPrefs.SetFloat ("last_audio_time", lastTime);
	}
}
