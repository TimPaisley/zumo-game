using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasManager : MonoBehaviour {

	private Text score;

	public Texture bounceUp;
	public Texture massUp;
	public Texture speedUp;

	public Texture bounceUpActive;
	public Texture massUpActive;
	public Texture speedUpActive;

	private RawImage[] slots;

	void Start () {
		score = GetComponentInChildren<Text> ();
		slots = GetComponentsInChildren<RawImage> ();

		DisableImages ();
	}

	public void UpdateSlots (Stack inv) {
		DisableImages ();
		Stack invClone = (Stack) inv.Clone ();

		for (int i = 0; i <= invClone.Count; i++) {
			string effect = (string) invClone.Pop ();

			if (effect == "BounceUp") {
				slots [i].texture = bounceUp;
			} else if (effect == "MassUp") {
				slots [i].texture = massUp;
			} else if (effect == "SpeedUp") {
				slots [i].texture = speedUp;
			}

			slots [i].enabled = true;
		}
	}

	private void DisableImages () {
		foreach (RawImage img in slots) {
			img.enabled = false;
		}
	}

	public void SetScore (int s) {
		score.text = s.ToString();
	}
}
