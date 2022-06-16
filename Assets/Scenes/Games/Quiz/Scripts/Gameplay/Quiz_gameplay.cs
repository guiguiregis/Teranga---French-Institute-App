using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kayfo;
using TMPro;
public class Quiz_gameplay : Singleton<Quiz_gameplay>
{
    public GameObject flashImage;

    public AudioSource m_feedbackSFX;
    public AudioClip m_correctSFX;
    public AudioClip m_incorrectSFX;


    //CHECK IF THE PLAYER HAS CLICKED ON THE RIGHT BUTTON
    public void CheckIfThePlayerHasClickedOnTheRightButton(bool is_correct, Image btn)
    {
        
        if(is_correct)
        {
            // FlashAnwserImage(true); 
            //Change the btn color
            if(StageManager.instance != null)
            {
                btn.color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].outlineColor;
                btn.transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].primaryColor;
                btn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.GOOD_ANWSER].textColor;
            }

            m_feedbackSFX.clip = m_correctSFX;  //assign the clip value
            m_feedbackSFX.Play();  //play the current clip
        }
        else
        {
            //DECREASED THE MINI GAME SCORE IF THE PLAYER HAS MADE A WRONG CHOICE
            Quiz_manager.Instance.DecreaseTheMiniGameScore();
            //Change the btn color
            if (StageManager.instance != null)
            {
                btn.color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].outlineColor;
                btn.transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].primaryColor;
                btn.transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.BAD_ANWSER].textColor;
            }


            m_feedbackSFX.clip = m_incorrectSFX;  //assign the clip value
            m_feedbackSFX.Play();  //play the current clip

        }
    
        //StartCoroutine(WaitBeforeGoingToNextTurn());
        StartCoroutine(IGotoNextTurn());
    }

    IEnumerator IGotoNextTurn()
    {
        yield return new  WaitForSeconds(0.5f);
        GotoNextTurn();
    }

    /// <summary>
    /// FLASH IMAGE COLOR BASED ON THE GIVEN ANWSER
    /// </summary>
    /// <param name="anwser">TO CHECK WHICH COLOR, WE SHOULD FLASH</param>
    public void FlashAnwserImage(bool anwser)
    {
        if (anwser)
        {
            StartCoroutine(FlashImage(Color.green));
        }
        else
        {
            StartCoroutine(FlashImage(Color.red));
        }
    }

    /// <summary>
    /// FLASH THE GIVEN COLOR AFTER A CERTAIN TIME 
    /// </summary>
    /// <param name="col">CHANGE THE IMAGE COLOR</param>
    /// <returns></returns>
    IEnumerator FlashImage(Color col)
    {
        col.a = 0.1f;
        float timeBeforeFlash = 0.1f;
        float flashDuration = 0.3f;
        yield return new  WaitForSeconds(timeBeforeFlash);
        flashImage.SetActive(true);
        flashImage.GetComponent<Image>().color = col;
        yield return new WaitForSeconds(flashDuration);
        flashImage.SetActive(false);

    }


    /// <summary>
    /// WAITING BEFORE PASSING TO THE NEXT TURN
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitBeforeGoingToNextTurn()
    {
        float timeBeforePassingToNextTurn = 0.1f;
        yield return new WaitForSeconds(timeBeforePassingToNextTurn);
        Quiz_manager.Instance.CheckIfTheNumberOfTurnsHasBeenReached();
    }


    public void GotoNextTurn()
    {
        Quiz_manager.Instance.CheckIfTheNumberOfTurnsHasBeenReached();
    }
}
