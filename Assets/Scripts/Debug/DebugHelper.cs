using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DebugHelper : MonoBehaviour {
	
	private Canvas canvas;

	void Start () {
		canvas = GetComponent<Canvas> ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.BackQuote)) {
			canvas.enabled = !canvas.enabled;
		} else if (Input.GetKeyDown (KeyCode.Tab)) {
			SceneManager.LoadScene ("Development");
		}
	}
}
