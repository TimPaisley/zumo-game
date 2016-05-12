using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugPlayer : MonoBehaviour {

	public PlayerController player;

	private Text[] modules;

	void Start () {
		modules = GetComponentsInChildren<Text> ();
	}

	void Update () {
		modules [1].text = player.gameObject.transform.position.ToString ();
		modules [2].text = "Last Bounce";
		modules [3].text = "Dash: " + player.GetComponent<PlayerDash> ().dashRemaining;
	}
}
