using UnityEngine;
using System.Collections;

public class FollowFox : MonoBehaviour {
    public Transform target;
    private NavMeshAgent agent;

	void Start () {
        agent = GetComponent<NavMeshAgent>();
	}

	void Update () {
		Vector3 an = target.eulerAngles * Mathf.Deg2Rad;
		Vector3 v3 = new Vector3 (Mathf.Sin(an.y), 0f, Mathf.Cos(an.y));
		Vector3 pos = target.transform.position + (3f * v3);
		

		agent.SetDestination(pos);
	}

}
