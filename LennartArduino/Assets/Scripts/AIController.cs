using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class AIController : MonoBehaviour {

    public float Speed = 1;
    public bool isInLight;
    public NavMeshAgent agent;
    public float arriveDistance;
    public Vector3 activeWanderPoint;
    public GameObject[] targets;
    public string targetTag;
    public float fov;
    public float randomMovementRadius;
    public float viewRange;

    private NavMeshPath path;
    private float timer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveToDestination(float speed, Vector3 targetPosition)
    {
        agent.speed = speed;
        agent.SetDestination(targetPosition);
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    public void NewWaypoint( float radius)
    {
        agent.isStopped = true;
        agent.isStopped = false;
        activeWanderPoint = RandomNavmeshLocation(radius);
        agent.SetDestination(activeWanderPoint);
        timer = 0;
    }

    public bool HasArrived()
    {
        float remainingDistance;
        if (agent.pathPending)
        {
            remainingDistance = float.PositiveInfinity;
        }
        else
        {
            remainingDistance = agent.remainingDistance;
        }
        return remainingDistance <= arriveDistance;
    }

    public GameObject LineOfSight(AIController owner)
    {
        RaycastHit hit;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                if (Vector3.Angle(targets[i].transform.position - owner.transform.position, owner.transform.forward) <= fov && Physics.Linecast(owner.transform.position, targets[i].transform.position, out hit) && hit.collider.transform == targets[i].transform && Vector3.Distance(hit.point, owner.transform.position) <= viewRange)
                {
                    return targets[i];
                }
            }
        }
        return null;
    }
}
