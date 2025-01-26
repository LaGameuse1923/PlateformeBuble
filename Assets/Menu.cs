using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Menu : MonoBehaviour
{
    public Animation animationplay;
    public bool PlayMenu = false;
    
    public VideoPlayer player;
    public Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(1);
        }
    }



    public void Play()
    {
        animationplay.animationPlayer();
        PlayMenu = true;
        animator.SetBool("PlayMenu", PlayMenu);
        Invoke("LancementAnimation", 0.6f);
    }

    public void LancementAnimation()
    {
        player.Play();
    }
}


