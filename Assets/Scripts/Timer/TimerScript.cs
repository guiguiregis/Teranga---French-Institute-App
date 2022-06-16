using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Kayfo;
public class TimerScript : Singleton<TimerScript>
{
    //DEFAULT TIMER 
    public float defaultTimer = 300f;
    public TextMeshProUGUI timerText;

    public bool m_timerRunning = false;
    private void Start()
    {
        //// Init();
        if (!StoryTellerCtrlr.Instance.m_canvas.gameObject.activeInHierarchy)   //if the story canvas is not activated then the timer should continue playing
        {
            Init();
            PauseCtrl.Instance.m_pauseBtn.SetActive(true);  //activated the pause btn
        }
    }
    public void Init()
    {

        // if(StoryTellerCtrlr.Instance != null)
        // {

        //     if(StoryTellerCtrlr.Instance.m_storyIntroduced)
        //     {
        //         ToggleTimer(true);
        //         Debug.Log("inside 0");
        //     }

        // }
        // else
        // {
        PauseCtrl.Instance.m_pauseBtn.SetActive(true);  //activated the pause btn
        ToggleTimer(true);
        // }


    }
    private void Update()
    {

        if(timerText == null)
        {
            if(GameObject.Find("TimerText") != null)
            {
                timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
            }
        }

        if(m_timerRunning && timerText != null)
        {
            if(StageManager.instance != null)
            {
                StartTimer(StageManager.instance.timer, timerText);
                StageManager.instance.timer -= Time.deltaTime;
            }
            else
            {
                StartTimer(defaultTimer, timerText);

                defaultTimer -= Time.deltaTime;
            }
        }
    }

    public void StartTimer(float timer, TextMeshProUGUI text)
    {

        if (timer > 0f)
        {
            string minutes = Mathf.Abs(Mathf.Floor(timer / 60)).ToString("00");
            string seconds = (timer % 60).ToString("00");
            string fraction = ((timer * 100) % 100).ToString("00");
            text.text = minutes + ":" + seconds;

        }
        if(StageManager.instance != null)
        {
            if (StageManager.instance.timer <= 0f)
            {
                StageManager.instance.LoadStageScreen(0);
            }
        }

    }

    public void ToggleTimer(bool status)
    {
        m_timerRunning = status;
    }

    IEnumerator IWaitBeforeStartTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        ToggleTimer(true);
    }
}
