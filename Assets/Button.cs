using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boutton : MonoBehaviour
{
    public int scene;
    public void NextScene()
    {
        Debug.Log("cc");
        SceneManager.LoadScene(scene);
        
    }
}
