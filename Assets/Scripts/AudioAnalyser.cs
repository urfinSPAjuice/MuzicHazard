using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioAnalyser : MonoBehaviour {

	[Header("Filter Parameters")]
	[Range(0f, 1f)]
	public float frequency;
	public Filter.FilterMode filterMode;
	public Filter.FilterOrder filterOrder;

	[Header("Averaged Parameters")]
	[Range(0.1f, 10f)]
	public float amplitude;
	[Range(0.1f, 1f)]
	public float smoothness;

	[Header("GUI")]
	public Slider sliderL,sliderR;
	public GraphRenderer graphL, graphR;

	[HideInInspector]
	public float valueL, valueR, sValueL, sValueR;

	float[] mLData, mRData;
	Filter mLFilter, mRFilter;
	int ix;

	float sampleRate;
	int bufferSize, bufferCount, channels;

	void Awake () {
		AudioSettings.GetDSPBufferSize (out bufferSize, out bufferCount);
		channels = (int)AudioSettings.driverCapabilities;
		sampleRate = AudioSettings.outputSampleRate;

		mLData = new float[bufferSize];
		mRData = new float[bufferSize];

		mLFilter = new Filter();
		mRFilter = new Filter();
		mLFilter.SetFilterMode(filterMode);
		mRFilter.SetFilterMode(filterMode);
		mLFilter.SetFilterOrder(filterOrder);
		mRFilter.SetFilterOrder(filterOrder);

		sliderL.minValue = 0f;
		sliderR.minValue = 0f;
		sliderL.maxValue = 1f;
		sliderR.maxValue = 1f;
	}

	void Start () {
		AudioSource mAudio = GetComponent<AudioSource>();
		int clipLength = mAudio.clip.samples * mAudio.clip.channels;
		float[] clipData = new float[clipLength];
		mAudio.clip.GetData(clipData, 0);
		int length = mAudio.clip.samples / bufferSize;
		Debug.Log (length);
		float[] dataL = new float[length];
		float[] dataR = new float[length];
		float[] buffer = new float[bufferSize * mAudio.clip.channels];
		for (int i = 0; i < length; i++) {
			// fill buffer
			for (int j = 0; j < bufferSize * mAudio.clip.channels; j++) {
				buffer[j] = clipData[i * bufferSize * mAudio.clip.channels + j];
			}
			Analyse (buffer, mAudio.clip.channels);
			dataL[i] = sValueL;
			dataR[i] = sValueR;
		}

		graphL.Redraw (dataL, .04f);
		graphR.Redraw (dataR, .04f);
	}

	void Update () {
		mLFilter.SetCutoff (frequency);
		mRFilter.SetCutoff (frequency);

		sliderL.value = sValueL;
		sliderR.value = sValueR;
	}

	void OnAudioFilterRead (float[] data, int channels) {
		Analyse (data, channels);
	}

	public void Analyse (float[] data, int channels) {
		valueL = valueR = 0f;
		for (int i = 0; i < data.Length; i += channels) {
			ix = i/channels;
			mLData[ix] = mLFilter.Process (data[i]);
			mRData[ix] = mRFilter.Process (data[i+1]);
			
			valueL += mLData[ix] * mLData[ix];
			valueR += mRData[ix] * mRData[ix];
		}
		valueL = 100f * amplitude * Mathf.Log10 (1f + 5f * Mathf.Sqrt (valueL) / bufferSize); //Mathf.Log10 (1f + 
		valueR = 100f * amplitude * Mathf.Log10 (1f + 5f * Mathf.Sqrt (valueR) / bufferSize);
		
		sValueL = (1 - smoothness) * sValueL + smoothness * valueL;
		sValueR = (1 - smoothness) * sValueR + smoothness * valueR;
	}
	
}