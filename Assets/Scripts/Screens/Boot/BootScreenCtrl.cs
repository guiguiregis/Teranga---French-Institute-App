using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootScreenCtrl : MonoBehaviour
{

    private Progress_struct progress_data;
    public float m_delay;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlaySplashScreen(m_delay));
    }


    public IEnumerator PlaySplashScreen(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        // Get progress data 
        if (PlayerPrefs.HasKey("progress"))
        {
            try
            {
                progress_data = ProgressManager.Instance.GetProgressStruct();
            }
            catch (Exception e)
            {
                SceneManager.LoadScene("Profile_setup_screen");
            }

            if (ProgressManager.Instance.m_persistentCharacterName.Get() == "") // Player not yet set
            {
                SceneManager.LoadScene("Profile_setup_screen");
            }
            else
            {
                SceneManager.LoadScene("Menu_screen");
            }

        }
        else
        {
            SceneManager.LoadScene("Profile_setup_screen");
        }

    }

 
}
