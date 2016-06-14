using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Loader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SceneManager.UnloadScene ("New Board");
		SceneManager.LoadScene ("New Board");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
