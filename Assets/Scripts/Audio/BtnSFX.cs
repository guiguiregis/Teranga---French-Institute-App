using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnSFX : MonoBehaviour
{
    public AudioSource m_myBtnFx;
    public AudioClip m_clickFx;

    public void ClickSound()
    {
        if(AudioManager.Instance.m_SFXActivated.Get())
        {
            m_myBtnFx.PlayOneShot(m_clickFx);
        }
    }
}
