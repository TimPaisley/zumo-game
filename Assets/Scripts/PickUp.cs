using UnityEngine;
using System.Collections;

public class PickUp : MonoBehaviour {

	private GameManager gm;
	private float startY;

	public string effect = "None";
	public float spinSpeed = 1.0f;
	public float floatSpeed = 1.0f;

	void Start () {
		gm = FindObjectOfType<GameManager> ();

		startY = transform.position.y;
	}

	void Update () {
		Float ();
		Spin ();
	}

	private void Float () {
		float newY = startY + 0.3f * Mathf.Sin (floatSpeed * Time.time);
		transform.position = new Vector3 (transform.position.x, newY, transform.position.z);
	}

	private void Spin () {
		transform.Rotate (0.0f, spinSpeed, 0.0f);
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player") {
			gm.PushToInventory (effect);
			Destroy (this.gameObject);
		}
	}

	public void SetEffect (string e) {
		effect = e;
	}
}
