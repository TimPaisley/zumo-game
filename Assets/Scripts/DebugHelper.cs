using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DebugHelper : MonoBehaviour {

	void Start () {
	
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.BackQuote)) {
			SceneManager.LoadScene ("Development");
		}
	}
}
