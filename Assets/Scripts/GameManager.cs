using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	public PlayerController[] players;

	public float bounceForce = 10.0f;

	void Start () {
		players = FindObjectsOfType<PlayerController> ();
	}
}
