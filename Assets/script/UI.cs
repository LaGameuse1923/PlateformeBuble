using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
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
        TextTimer.text = "Temp : " + Timer;
        StartCoroutine(WaitAndPrint());
        videoPlayer.Play();
        
    }

    // Update is called once per frame
    void Update()
    {
        TextTimer.text = "Times: " + Timer;
        BarreDePopulariter.fillAmount = nombreErreurRestante / nombreErreur;
        if (textManager is not null)
        {
            nombreErreurRestante = textManager.GetError();
            NombreErreur.text = nombreErreurRestante + " Typos Left";
        }
        else
        {
            NombreErreur.text = "999 left!";
        }
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

