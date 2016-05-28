using UnityEngine;
using System.Collections;

public class WigglingText : MonoBehaviour {
    public float maxRotation = 20;

    private Vector3 initialRotation;
    
	void Start () {
        initialRotation = transform.localEulerAngles;
	}
	
	void Update () {
        transform.localEulerAngles = initialRotation + new Vector3(0, Mathf.Sin(Time.fixedTime) * maxRotation, 0);
	}
}
