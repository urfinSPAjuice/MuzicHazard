using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class GraphRenderer : MonoBehaviour {

	public Transform min, max;
	LineRenderer lRenderer;

	// Use this for initialization
	void Start () {
		lRenderer = GetComponent<LineRenderer>();
		lRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		lRenderer.receiveShadows = false;
		lRenderer.useWorldSpace = true;
		lRenderer.SetWidth (0.01f, 0.01f);
		//lRenderer.SetColors (Color.green, Color.green);
		lRenderer.sortingOrder = 10;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Redraw (float[] data, float maxY) {
		lRenderer.SetVertexCount (data.Length);
		Vector3 offset = Vector3.zero;
		float offX = (max.position.x - min.position.x) / (data.Length - 1);
		float kY = (max.position.y - min.position.y) / maxY;
		for (int i = 0; i < data.Length; i++) {
			offset.y = kY * data[i];
			lRenderer.SetPosition (i, min.position + offset);
			offset.x += offX;
		}
	}
}
