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
		modules [2].text = "Speed: " + player.speed;
		modules [3].text = "Dash: " + player.GetComponent<PlayerDash> ().dashRemaining;
		modules [4].text = "Last Bounce (NOT WORKING)";
	}
}
