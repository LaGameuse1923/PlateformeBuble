using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Video;

public class UI : MonoBehaviour
{
    private bool coroutine = true;

    public float nombreErreur = 5;
    public float nombreErreurRestante = 5;
    public TextMeshProUGUI NombreErreur;

    public float Timer = 300;
    public TextMeshProUGUI TextTimer;

    public Image BarreDePopulariter;
    public VideoPlayer videoPlayer;


    // Start is called before the first frame update

    void Start()
    {
        TextTimer.text = "Temp : " + Timer.ToString();
        StartCoroutine(WaitAndPrint());
        videoPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        NombreErreur.text = nombreErreurRestante + "/" + nombreErreur.ToString();
        TextTimer.text = "Temp : " + Timer.ToString();
        BarreDePopulariter.fillAmount = nombreErreurRestante / nombreErreur;
    }

    IEnumerator WaitAndPrint()
    {
        while (coroutine)
        {
            
            if (Timer == 0 || nombreErreurRestante == 0)
            {
                coroutine = false;
                Debug.Log("fin");
            }
            else {
                Timer--;
            }
            yield return new WaitForSeconds(1);
        }

    }
}

