using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CheckActivatedLetters : MonoBehaviour
{

    public Image m_buttonImage;
    public TextMeshProUGUI m_letterText;


    // Update is called once per frame
    void Update()
    {
        if(m_letterText.text != "")
        {
            m_buttonImage.gameObject.SetActive(true);
        }
        else
        {
            m_buttonImage.gameObject.SetActive(false);
        }
    }
}
