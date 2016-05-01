using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	// Update is called once per frame
	void Update () {
        Vector3 tran = new Vector3(0,45,0);
        transform.Rotate(tran * Time.deltaTime);
	}
}
