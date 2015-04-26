using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private const float movementFactor = 0.05f;
	private const float movementSmooth = 0.03f;

	public Transform control;

	public ParticleSystem leftGun, rightGun;

	public AudioAnalyser analyser;

	private Vector3 lerpPosition, cameraOffset, cross;
	private float angle, maxX, maxY;

	// Use this for initialization
	void Start () {
		cameraOffset = Camera.main.transform.position;

		maxX = Camera.main.ScreenToWorldPoint (new Vector2(Screen.width, 0)).x - GetComponent<SpriteRenderer>().bounds.size.x;
		maxY = Camera.main.ScreenToWorldPoint (new Vector2(0, Screen.height)).y - GetComponent<SpriteRenderer>().bounds.size.y;
	}
	
	// Update is called once per frame
	void Update () {
		// keyboard controls
		//lerpPosition = transform.position;
		if (Input.GetKey(KeyCode.W)) {
			lerpPosition += movementFactor * Vector3.up;
		}
		if (Input.GetKey(KeyCode.A)) {
			lerpPosition += movementFactor * Vector3.left;
		}
		if (Input.GetKey(KeyCode.S)) {
			lerpPosition += movementFactor * Vector3.down;
		}
		if (Input.GetKey(KeyCode.D)) {
			lerpPosition += movementFactor * Vector3.right;
		}
		lerpPosition.x = Mathf.Clamp (lerpPosition.x, -maxX, maxX);
		lerpPosition.y = Mathf.Clamp (lerpPosition.y, -maxY, maxY);
		transform.position = Vector3.Lerp (transform.position, lerpPosition, movementSmooth);

		// aim
		control.position = Camera.main.ScreenToWorldPoint (Input.mousePosition) - cameraOffset;

		// rotation
		angle = Vector2.Angle (control.position - transform.position, Vector2.up);
		cross = Vector3.Cross(control.position - transform.position, Vector2.up);		
		if (cross.z > 0) angle = 360 - angle;
		transform.localRotation = Quaternion.Euler (0f, 0f, angle);

		// shooting
		leftGun.emissionRate = 1 + 50f * analyser.sValueL;
		rightGun.emissionRate = 1 + 50f * analyser.sValueR;
		leftGun.startSize = 0.2f + 0.4f * analyser.sValueL;
		rightGun.startSize = 0.2f + 0.4f * analyser.sValueR;
	}
}
