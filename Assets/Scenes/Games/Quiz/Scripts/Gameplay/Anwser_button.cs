using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Anwser_button : MonoBehaviour
{
    public bool isCorrect;


    private void Start()
    {
        //set default btn color 
        if (StageManager.instance != null)
        {
            transform.GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].outlineColor;
            transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].primaryColor;
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].textColor;
        }
    }

    public void CheckAnswer()
    {
        if(!Quiz_manager.Instance.m_is_choice_selected)
        {
            Quiz_manager.Instance.m_is_choice_selected = true;
            FindObjectOfType<Quiz_gameplay>().CheckIfThePlayerHasClickedOnTheRightButton(isCorrect, transform.GetComponent<Image>());
        }
    }
}
