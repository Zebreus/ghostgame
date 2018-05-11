using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRoomTrigger : MonoBehaviour
{
    public bool isLit;
    public ConnectorPath[] exitPoint;
    private Light[] pointLights;
    private BoxCollider[] colliders;
    public KeyCode input;
    public bool RaveMode;
    public bool locked;

    private bool colorChanged;

    private void Start()
    {
        colliders = GetComponents<BoxCollider>();
        pointLights = GetComponentsInChildren<Light>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(input) && !locked)
        {
            isLit = !isLit;
        }

        if (isLit)
        {

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = true;
            }
            for (int i = 0; i < pointLights.Length; i++)
            {
                pointLights[i].enabled = true;
                if (RaveMode && !colorChanged)
                {
                    Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
                    pointLights[i].color = color;
                    pointLights[i].intensity = 12;
                }
            }
            colorChanged = true;
        }
        else
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = false;
            }
            for (int i = 0; i < pointLights.Length; i++)
            {
                pointLights[i].enabled = false;
                pointLights[i].intensity = 7.05f;
            }
            colorChanged = false;
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<GhostAI>())
        {
            other.GetComponent<GhostAI>().isInLight = true;
            Transform wayPoint = null;
            float distance = float.MaxValue;
            for (int i = 0; i < exitPoint.Length; i++)
            {
                if (!exitPoint[i].TriggerArea.isLit && Vector3.Distance(exitPoint[i].waypoint.position, other.transform.position) < distance)
                {
                    distance = Vector3.Distance(exitPoint[i].waypoint.position, other.transform.position);
                    wayPoint = exitPoint[i].waypoint;
                }

            }
            if (wayPoint != null)
            {
                other.GetComponent<GhostAI>().agent.SetDestination(wayPoint.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GhostAI>())
        {
            other.GetComponent<GhostAI>().isInLight = false;
        }
    }
}
