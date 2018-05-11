using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Generator : MonoBehaviour
{
    public LightRoomTrigger[] triggerRooms;
    public int activeRooms;
    public int maxLitRooms;
    public bool dead = false;
    private GameObject[] peasants;
    private int peasantCount;

    private void Start()
    {
        peasants = GameObject.FindGameObjectsWithTag("Peasant");
    }

    private void Update()
    {

        peasantCount = 0;
        for (int i = 0; i < peasants.Length; i++)
        {
            if (peasants[i] != null)
            {
                peasantCount++;
            }
        }

        maxLitRooms = peasantCount / 2;

        if (peasantCount <= 1)
        {
            dead = true;
        }

        activeRooms = 0;
        for (int i = 0; i < triggerRooms.Length; i++)
        {
            if (triggerRooms[i].isLit)
            {
                activeRooms++;
            }
        }
        if (activeRooms > maxLitRooms)
        {
            StartCoroutine(PowerOut(1));
        }
    }

    public IEnumerator PowerOut(int timeMSek)
    {
        int counter = 0;
        while (counter < timeMSek)
        {
            for (int i = 0; i < triggerRooms.Length; i++)
            {
                triggerRooms[i].isLit = false;
                triggerRooms[i].locked = true;
            }
            yield return new WaitForSeconds(1f);
            counter++;
        }
        for (int i = 0; i < triggerRooms.Length; i++)
        {
            triggerRooms[i].locked = false;
        }
    }
}
