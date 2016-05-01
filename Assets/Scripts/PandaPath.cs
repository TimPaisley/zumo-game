using UnityEngine;
using System.Collections;

public class PandaPath : MonoBehaviour {

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    public GameObject playerFox;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        GotoNextPoint();
    }


    void GotoNextPoint()
    {
        if (points.Length == 0)
            return;
        agent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;
    }


    void Update()
    {
     
        float range = Vector3.Distance(this.transform.position,playerFox.transform.position);
   
        if (range < 7)
        {
            agent.SetDestination(playerFox.transform.position);
        }
        if (agent.remainingDistance < 0.5f) { GotoNextPoint(); }
    }
}
