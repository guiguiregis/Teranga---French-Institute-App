using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kayfo;
using UnityEngine.SceneManagement;
public class PauseCtrl : Singleton<PauseCtrl>
{

    public GameObject m_pauseBtn;
    public GameObject m_pausePanel;
    private void Update()
    {
        //{
        if (GameObject.Find("Timer") == null && m_pauseBtn.activeInHierarchy)
        {
            m_pauseBtn.SetActive(false);
        }
        //}
    }


    public void TogglePauseMenu()
    {
        m_pausePanel.SetActive(!m_pausePanel.activeInHierarchy);
        FindObjectOfType<TimerScript>().ToggleTimer(!m_pausePanel.activeInHierarchy); // Popup On, Timer Off
        var tutorialGo = GameObject.Find("Tutorial");

        if (tutorialGo == null)
        {
            return;
        }

        if (m_pausePanel.activeInHierarchy)  //if the pause panel is activated in hierachy
        {
            tutorialGo.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            tutorialGo.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void GoToModuleScr()
    {
            m_pausePanel.SetActive(false);
            SceneManager.LoadScene("Module_screen", LoadSceneMode.Single);
    }
}
