using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using TMPro;

public class CloudItem : MonoBehaviour , IPointerClickHandler
{
    public bool m_clicked = false;
    // Start is called before the first frame update
    void Start()
    {
        //set default btn color 
        if (StageManager.instance != null)
        {
            transform.GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].outlineColor;
            transform.GetChild(0).GetComponent<Image>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].primaryColor;
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().color = StageManager.instance.m_btnColors[(int)StageManager.BTN_COLOR.DEFAULT].textColor;
        }
    }

    // Update is called once per frame

    public void OnPointerClick(PointerEventData eventData) {

     if(!m_clicked)
     {
         m_clicked = true;

        int index = transform.GetSiblingIndex();
        string word = transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text;
        FindObjectOfType<WC_LevelManager>().CheckWord(index , word);
        
     }
       


    }
}
