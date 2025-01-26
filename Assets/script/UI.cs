using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    private bool coroutine = true;
    public TextManager textManager;
    public float nombreErreur = 5;
    public float nombreErreurRestante = 5;
    public TextMeshProUGUI NombreErreur;
    public int scene;

    public float Timer = 300;
    public TextMeshProUGUI TextTimer;

    public Animator animator;
    public Image BarreDePopulariter;
    public VideoPlayer videoPlayer;

    public bool BF = false;
    public bool MF = false;
    public bool BE = false;


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
        NombreErreur.text = nombreErreurRestante + " Typos Left".ToString();
        TextTimer.text = "Times: " + Timer.ToString();
        BarreDePopulariter.fillAmount = nombreErreurRestante / nombreErreur;
        if (textManager != null)
            nombreErreurRestante = textManager.GetError();
    
        

        //Debug.Log(nombreErreurRestante);
    }

    IEnumerator WaitAndPrint()
    {
        while (coroutine)
        {
            
            if (Timer == 0 || nombreErreurRestante == 0)
            {
                coroutine = false;

                if (nombreErreurRestante == 0)
                {
                    BF = true;
                    animator.SetBool("BF",BF );
                    SceneManager.LoadScene(scene);
                }else if (nombreErreurRestante > 5)
                {
                    MF = true;
                    animator.SetBool("MF", MF);
                }else if (nombreErreurRestante > 7)
                {
                    BE = true;
                    animator.SetBool("BE", BE);
                }


                
            }
            else {
                Timer--;
            }
            yield return new WaitForSeconds(1);
        }

    }
}

