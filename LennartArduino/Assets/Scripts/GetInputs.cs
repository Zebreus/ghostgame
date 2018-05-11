using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetInputs : MonoBehaviour
{
    public ArduinoConnector communicator;
    public Generator generator;

    private bool initialized;
    private float timer;
    private int oldPower;

    private void Start()
    {
        communicator = GetComponent<ArduinoConnector>();
    }

    void FixedUpdate()
    {
        if (generator.dead)
        {
            StartCoroutine(Waiting());
        }
        if (timer >= 2 && !initialized)
        {
            initialized = true;
            communicator.WriteToArduino("N8\n");
        }
        else if (!initialized)
        {

        }
        {
            timer += Time.deltaTime;
        }

        if (initialized)
        {
            if (oldPower != Mathf.Clamp(generator.maxLitRooms, 2, 7))
            {
                communicator.WriteToArduino("P" + Mathf.Clamp(generator.maxLitRooms, 2, 7) + "\n");
                oldPower = Mathf.Clamp(generator.maxLitRooms, 2, 7);
            }

            communicator.WriteToArduino("S\n");
            string inPut = communicator.ReadFromArduino(50);
            Debug.Log(inPut);
            if (inPut != null && inPut != "INVALID")
            {
                char[] charArray = inPut.ToCharArray();
                for (int i = 11; i < charArray.Length; i++)
                {
                    //Debug.Log(charArray[i]);
                    if (charArray[i] == '1')
                    {
                        generator.triggerRooms[i - 11].isLit = true;
                    }
                    if (charArray[i] == '0')
                    {
                        generator.triggerRooms[i - 11].isLit = false;
                    }
                }
            }
        }
    }

    private IEnumerator Waiting()
    {
        int counter = 0;
        communicator.WriteToArduino("O\n");
        while (counter == 0)
        {
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene("GameOver");
        }
    }
}
