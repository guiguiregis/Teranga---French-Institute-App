using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenuCtrl : MonoBehaviour
{
    public GameObject m_options_panel;
    public GameObject m_sound_configs_panel;
    public GameObject m_SFXBtn;
    public GameObject m_musicBtn;


    public void ToggleMusicBtn()
    {
        if(AudioManager.Instance.m_musicActivated.Get())
        {
            m_musicBtn.transform.GetChild(1).gameObject.SetActive(true);
            AudioManager.Instance.m_musicActivated.Set(false);
            AudioManager.Instance.gameObject.GetComponent<AudioSource>().Stop();
        }
        else
        {
            m_musicBtn.transform.GetChild(1).gameObject.SetActive(false);
            AudioManager.Instance.m_musicActivated.Set(true);
            AudioManager.Instance.gameObject.GetComponent<AudioSource>().Play();
        }
    }

    public void ToggleSoundBtn()
    {
        if (AudioManager.Instance.m_SFXActivated.Get())
        {
            m_SFXBtn.transform.GetChild(1).gameObject.SetActive(true);
            AudioManager.Instance.m_SFXActivated.Set(false);
        }
        else
        {
            m_SFXBtn.transform.GetChild(1).gameObject.SetActive(false);
            AudioManager.Instance.m_SFXActivated.Set(true);
        }

    }
}
