using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TutoCtrlr : MonoBehaviour
{

    public GameObject m_trigger;
    public GameObject m_popup;
    public GameObject m_blocker;
    public GameObject m_blocsContainer;
    public Button m_leftNav;
    public Button m_rightNav;

    int m_currentBlocIndex = 0;
    int m_prevBlocIndex = 1;
    int m_blocsSize = 2;
    // Start is called before the first frame update
    void Start()
    {
        m_blocsSize = m_blocsContainer.transform.childCount;

        if(StageManager.instance == null)
        {
            FindObjectOfType<TimerScript>().m_timerRunning = true;
        }
    }

    public void Navigate(int dir)
    {
        m_prevBlocIndex = m_currentBlocIndex;
        m_currentBlocIndex = m_currentBlocIndex + dir;
        // if( m_currentBlocIndex < 0) m_currentBlocIndex = (m_blocsSize - 1);
        // if( m_currentBlocIndex >= m_blocsSize) m_currentBlocIndex = 0;

        if(m_currentBlocIndex == 0)
        {
            m_leftNav.interactable = false;
            m_rightNav.interactable = true;
        }
        if(m_currentBlocIndex == (m_blocsSize - 1)) 
        {
            m_rightNav.interactable = false;
            m_leftNav.interactable = true;
        }

        ActivateBloc();

    }

    void ActivateBloc()
    {
        GameObject prevBloc = m_blocsContainer.transform.GetChild(m_prevBlocIndex).gameObject;
        prevBloc.SetActive(false);
        
        GameObject nextBloc = m_blocsContainer.transform.GetChild(m_currentBlocIndex).gameObject;
        nextBloc.SetActive(true);

    }


    public void ToggleTutorial(bool status)
    {

        FindObjectOfType<TimerScript>().ToggleTimer(!status); // Popup On, Timer Off

        m_trigger.SetActive(!status);
        m_popup.SetActive(status);
        m_blocker.SetActive(status);

        if(!status) // Hiding popup : Reset All
        {
            m_prevBlocIndex = m_currentBlocIndex;
            m_currentBlocIndex = 0;
         
            ActivateBloc();

            m_leftNav.interactable = false;
            m_rightNav.interactable = true;

        }

    }

  
}
