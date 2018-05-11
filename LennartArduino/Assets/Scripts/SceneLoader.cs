using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

    public void OnButtonPress()
    {
        SceneManager.LoadScene("Scene");
    }
}
