using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PeasantAI : AIController {

    public float runAwaySpeed;

    private Transform startTransform;

    private void Start()
    {
        targets = GameObject.FindGameObjectsWithTag(targetTag);
        activeWanderPoint = transform.position;
        agent.speed = Speed;
    }

    private void Update()
    {
        GameObject target = LineOfSight(this);
        if (!target)
        {
            agent.speed = Speed;
            if (HasArrived())
            {
                NewWaypoint(randomMovementRadius);
            }
        }
        else
        {
            RunFrom(target.transform);
        }
    }

    public void RunFrom(Transform target)
    {
        agent.speed = runAwaySpeed;
        startTransform = transform;

        transform.rotation = Quaternion.LookRotation(transform.position - target.position);

        Vector3 runTo = transform.position + transform.forward * 1;

        NavMeshHit hit;
        
        NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Default"));
        
        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;
        agent.SetDestination(hit.position);
    }
}
