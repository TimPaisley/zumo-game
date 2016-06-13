using UnityEngine;
using System.Collections;
using System.Linq;

public class objectScatterer : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		GameObject[] scatterObjects = GameObject.FindGameObjectsWithTag ("Scatter Object");
		GameObject[] scatterSpawns = GameObject.FindGameObjectsWithTag ("Scatter Spawn");

		var randomObjects = scatterObjects.OrderBy (_ => UnityEngine.Random.value).ToArray();

		for (int i = 0; i < scatterSpawns.Length; i++) {
			randomObjects[i].transform.position = scatterSpawns[i].transform.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
