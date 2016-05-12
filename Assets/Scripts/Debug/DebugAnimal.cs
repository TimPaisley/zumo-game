using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugAnimal : MonoBehaviour {

	public AnimalController animal;

	private Text[] modules;

	void Start () {
		modules = GetComponentsInChildren<Text> ();
	}

	void Update () {
		modules [1].text = animal.gameObject.transform.position.ToString ();
		modules [2].text = "Last Bounce";
		modules [3].text = "Dash: " + animal.GetComponent<AnimalDash> ().dashRemaining;
	}
}
