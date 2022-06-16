using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ModuleBtnCtrl : MonoBehaviour  
{

    public List<Sprite> m_trophiesList;
    public Image m_background;
    public Image m_progressFill;
    public TextMeshProUGUI m_percentageText;
    public Image m_trophy;

    public GameObject m_progressView;
    public GameObject m_startView;



    public void OnPointerClick()
    {

        string module_name = StageManager.instance.m_modulesDisplayName.Where(m => m.dislpayName.Equals(transform.Find("Title").GetComponent<TextMeshProUGUI>().text)).FirstOrDefault().fileName;   //get the module name which is written inside the json file
        if(module_name != "En développement")
        {
            StageManager.instance.SetString("current_module_name", module_name);
            StageManager.instance.LoadModuleScreen();
        }

        StoryTellerCtrlr.Instance.InitializeStorytelling();
    }

    public void SetBtnUI(int background_index, int progress, Color color)
    {
        // if (background_index != -1)
        // {
        // }
        // else
        // {
        //     // m_background.sprite = null;
        //     m_percentageText.text = "- %";
        // }
        bool onProgress = progress > 0 ? true : false;
        m_startView.SetActive(!onProgress);
        m_progressView.SetActive(onProgress);
        // transform.Find("Button").GetComponent<Button>().interactable = onProgress;
        // transform.Find("Button").GetComponent<Image>().raycastTarget = onProgress;

        m_percentageText.text = Mathf.Round(progress).ToString() + "%";

        m_trophy.sprite = m_trophiesList[background_index + 1];
        m_progressFill.color = color;
        m_progressFill.fillAmount = (float)progress/100f;
        

    }
    // Start is called before the first frame update

}
