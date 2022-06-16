using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class FlatCharacterSetupCtrlr : MonoBehaviour
{
    // Start is called before the first frame update

    int m_activeStepIndex = 0;
    string m_characterName = "-";
    string m_companyName = "-";
    int m_selectedCharacterIndex = 0;
    public GameObject m_charactersContainer; 
    public GameObject m_selectedCharactersContainer; 
    public GameObject m_companyItem; 
    public GameObject m_stepsContainer; 

    public TextMeshProUGUI m_headerText;

    public Button m_prevBtn;
    public Button m_nextBtn;

    public TMP_InputField m_characterNameField;
    public TMP_InputField m_companyNameField;

    public List<Sprite> m_nextBtnSprites;

    bool m_hasSelected = false;

    List<string> m_headerTextes = new List<string>() {"Choisissez votre personnage", "", ""};
    void Start()
    {
        m_prevBtn.interactable = false;
        m_prevBtn.gameObject.SetActive(false);
        m_nextBtn.interactable = false;
        m_headerText.text = m_headerTextes[0];

    }

    public void SelectStep(int index)
    {
        m_stepsContainer.transform.GetChild(m_activeStepIndex).GetComponent<FlatStepItemCtrlr>().Toggle(false);

        m_stepsContainer.transform.GetChild(index).GetComponent<FlatStepItemCtrlr>().Toggle(true);
        m_activeStepIndex = index;

    }

    public void NextStep()
    {
        int nextStepIndex = m_activeStepIndex + 1;

        if(nextStepIndex > 2) // From CompanyName To BD
        {
            if(m_companyName.Trim() != "" && m_companyName.Trim().Length > 1)
            {
                GotoMenu();
            }
            else
            {
                AlertInput(m_companyNameField);
            }
        }
        else if(nextStepIndex == 2) // From Name To CompanyName
        {
            FocusField(m_companyNameField);

            if(m_characterName.Trim() != "" && m_characterName.Trim().Length > 1)
            {

                SelectStep(nextStepIndex);

                m_nextBtn.transform.GetComponent<Image>().sprite = m_nextBtnSprites[1];
                m_nextBtn.transform.GetComponent<Image>().color =  new Color(0.56f, 0.74f, 0.07f);

                // Header 
                m_headerText.text = m_headerTextes[nextStepIndex];
            }
            else
            {
                AlertInput(m_characterNameField);
            }
        }
        else
        {
            FocusField(m_characterNameField);
            SelectStep(nextStepIndex);
            m_headerText.text = m_headerTextes[nextStepIndex];

            m_nextBtn.transform.GetComponent<Image>().sprite = m_nextBtnSprites[0];
            m_nextBtn.transform.GetComponent<Image>().color =  new Color(1f, 1f, 1f);
        }

            // // Header 
            // m_headerText.text = m_headerTextes[nextStepIndex];

        if(nextStepIndex > 0)
        {
            m_prevBtn.gameObject.SetActive(true);
            m_prevBtn.interactable = true;
        }
            

        // Display Only selected character avatar
        SetSelectedCharacter();

       

    }

    public void AlertInput(TMP_InputField field)
    {
        StartCoroutine(IAlertInput(field));
    }

    void FocusField(TMP_InputField field)
    {
        field.Select();
        field.ActivateInputField(); 
    }

    IEnumerator IAlertInput(TMP_InputField field)
    {
        field.transform.GetComponent<Image>().color = new Color(0.962f, 0.485f, 0.485f);

        yield return new WaitForSeconds(1.0f);

        field.transform.GetComponent<Image>().color = new Color(1f, 1f, 1f);
    }

    public void PrevStep()
    {
        int prevStepIndex = m_activeStepIndex - 1;
        SelectStep(prevStepIndex);

        if(prevStepIndex == 0)
        {
            m_prevBtn.gameObject.SetActive(false);
            m_prevBtn.interactable = false;
        }
           

         // Header 
        m_headerText.text = m_headerTextes[prevStepIndex];


        if(prevStepIndex < 2)
        {
            m_nextBtn.transform.GetComponent<Image>().sprite = m_nextBtnSprites[0];
            m_nextBtn.transform.GetComponent<Image>().color =  new Color(1f, 1f, 1f);
        }
    }

    public void SetSelectedCharacter()
    {
        int it = 0;
        foreach (Transform character in m_selectedCharactersContainer.transform)
        {
            character.gameObject.SetActive(it == m_selectedCharacterIndex);
            it++;
        }

    }
    public void SelectCharacter(int index)
    {
        int it = 0;
        foreach (Transform character in m_charactersContainer.transform)
        {
            character.GetComponent<FlatCharacterItemCtrlr>().Toggle(it == index);
            it++;
        }

        m_selectedCharacterIndex = index;


        m_hasSelected = true;
        m_nextBtn.interactable = true;

        NextStep();
    }

    public void UpdateSelectedCharacterName()
    {   
        m_characterName = m_characterNameField.text;
        m_selectedCharactersContainer.transform.GetChild(m_selectedCharacterIndex).GetComponentInChildren<TextMeshProUGUI>().text = m_characterName;
    }

    public void UpdateCompanyName()
    {   
        m_companyName = m_companyNameField.text;
        m_companyItem.GetComponentInChildren<TextMeshProUGUI>().text = m_companyName;
    }

    void GotoMenu()
    {
        ProgressManager.Instance.m_persistentCharacterIndex.Set(m_selectedCharacterIndex);
        if(m_characterName == "") m_characterName = "Default Name";
        if(m_companyName == "") m_companyName = "TerangaCorp";
        ProgressManager.Instance.m_persistentCharacterName.Set(m_characterName);
        ProgressManager.Instance.m_persistentCompanyName.Set(m_companyName);
        
        // Debug.Log(m_selectedCharacterIndex);

        // SceneManager.LoadScene("Menu_screen");
        SceneManager.LoadScene("BD_engine_screen");

    }
 
}
