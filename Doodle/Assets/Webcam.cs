using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Webcam : MonoBehaviour
{
    [SerializeField] private RawImage img  = default;
    public WebCamTexture webCam; 
    public IntPtr pt;


    void Start()
    {
        webCam = new WebCamTexture();
        if(!webCam.isPlaying)
            webCam.Play();
        img.texture = webCam;
        
    }


    void Update()
    {
                    

    }

}

