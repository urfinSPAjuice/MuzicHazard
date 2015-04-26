using UnityEngine;
using System.Collections;

public class Enemie : MonoBehaviour {

	public const int devide_count = 2;
	public const int max_count = 2;
	public const float speed = 4f;
	public const float playerAttraction = 1f;

	int _iteration = 0;
	public int iteration {
		get {
			return _iteration;
		}
		set {
			_iteration = value;
			transform.parent.localScale = Vector3.one * (1f / (1 + _iteration));
			transform.parent.GetComponent<Rigidbody2D>().velocity += new Vector2 (Random.Range(-speed, speed), Random.Range(-speed, speed));
		}
	}

	Transform player;

	void Start () {
		player = GameObject.Find ("Starship").transform;
	}


	void Update () {
		transform.parent.GetComponent<Rigidbody2D>().AddForce ( - (transform.position - player.position).normalized * playerAttraction);
		if (transform.position.sqrMagnitude >= 100f) {
			DestroyImmediate (transform.parent.gameObject);
		}
	}

	void OnParticleCollision (GameObject go) {
		if (iteration <= max_count) { 
			DuplicateSelf (max_count);
		}
		DestroyImmediate (transform.parent.gameObject);
	}

	void DuplicateSelf (int times) {
		for (int i = 0; i < times; i++)
			Instantiate(transform.parent.gameObject).transform.GetChild(0).GetComponent<Enemie>().iteration = iteration + 1;
	}
}
