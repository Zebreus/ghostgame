using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class GhostAI : AIController
{
    public float leaveDistance;

    private GameObject target;

    private void Start()
    {
        targets = GameObject.FindGameObjectsWithTag(targetTag);
        activeWanderPoint = transform.position;
        agent.speed = Speed;
    }

    private void Update()
    {
        target = LineOfSight(this);
        if (!target)
        {
            if (HasArrived())
            {
                NewWaypoint(randomMovementRadius);
            }
        }
        else if (Vector3.Distance(target.transform.position, transform.position) > leaveDistance)
        {
            target = null;
        }
        else if (target)
        {
            if (!isInLight)
            {
                agent.SetDestination(target.transform.position);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == targetTag)
        {
            Destroy(collision.gameObject);
        }
    }
}
