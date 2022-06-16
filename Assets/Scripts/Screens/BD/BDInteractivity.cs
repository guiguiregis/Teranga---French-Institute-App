using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BDInteractivity : MonoBehaviour
{
    public GameObject characters_carousel;
    public GameObject characters_container;
    public GameObject ui_navigator;
    public GameObject controls_activator;
    public GameObject left_arrow;
    public GameObject right_arrow;
    public List<Frame_ui> frames_ui;
    public GameObject Name_step;
    public GameObject Company_step;
    public GameObject bd;
    public List<Sprite> bd_frames;
    public InputField m_nameInputField;
    public Text m_nameInputValue;
    public InputField m_companyInputField;
    public Text m_companyInputValue;
    
    //This below is used to define the last active frame of the comic : Init to first frame (0)
    int previous_active_frame = 0; 
    int active_character_index = 0; // Male
    string player_name = ""; // 
    string company_name = ""; // 
    public float slide_speed = 4;
    float carousel_items_width;

    private Progress_struct progress_data;
    
    Vector3 target;
    bool switching_character = false;
    // Start is called before the first frame update
    void Start()
    {
        // Get progress data at any time we load this screen
        progress_data = ProgressManager.Instance.GetProgressStruct();      
        // Init first partner : This ensures if user did set or not his data before leaving the App
        // in order to know on which screen we boot (BDSCREEN or MENUSCREEN)
        // Progress_partner_struct first_partner =  new Progress_partner_struct();
        // progress_data.partners.Add(first_partner);
 
        carousel_items_width = characters_carousel.GetComponent<SpriteRenderer>().bounds.size.x;

        SetCharacter();
    }

    void Update() {
        
        
        if (switching_character)
        {
           
            characters_container.transform.position = Vector3.Lerp(characters_container.transform.position , target , slide_speed * Time.deltaTime);
            
            if (FindObjectOfType<BDReaderEngine>().TargetReached(characters_container.transform.position , target) == true)
                {
                    switching_character = false;
                }

        }

    }

    public void SelectCharacter(int dir)
    {

            // Set selected character 

            if((active_character_index == 0 && dir == -1) || (active_character_index == 1 && dir == 1) )
            {

              if (active_character_index == 1)
              {
               
                   left_arrow.SetActive(false);
                   right_arrow.SetActive(true);


                  active_character_index = 0;

              }
              else
              {
              
                   left_arrow.SetActive(true);
                   right_arrow.SetActive(false);

                  active_character_index = 1;

              }
             
              float margin = carousel_items_width * -(dir);
    
              target = characters_container.transform.position;
          
              target.x -= margin ;

              switching_character = true;

            }
       
            // characters_container.transform.position = Vector3.Lerp(characters_container.transform.position , dest , 1f);


    }

    public void SetCharacter()
    {

        int index = ProgressManager.Instance.m_persistentCharacterIndex.Get();
        // Debug.Log(index);
        // Set selected character 
        active_character_index = index;
        characters_container.transform.GetChild(1-index).gameObject.SetActive(false); // 1 - 0 = 1 / 1 - 1 = 0
        characters_container.transform.GetChild(index).gameObject.SetActive(true);

        // Change BD
        bd.transform.GetComponent<SpriteRenderer>().sprite = bd_frames[active_character_index];


    }


    //This Activate or deactivate UI for a frame when this one is shown
    public void SetFrameUI(int frame_index)
    {

         // Deactivate previous 
         for (int i = 0; i < frames_ui.Count; i++)
         {
             if(frames_ui[i].frame_index == previous_active_frame)
             {
                 List<GameObject> items = frames_ui[i].items;

                 for (int j = 0; j < items.Count; j++)
                 {
                     items[j].SetActive(false);
                 }

                 break;
             }
         }

         // Activate current

         for (int i = 0; i < frames_ui.Count; i++)
         {
             if(frames_ui[i].frame_index == frame_index)
             {
                 List<GameObject> items = frames_ui[i].items;

                 for (int j = 0; j < items.Count; j++)
                 {
                     items[j].SetActive(true);
                 }
                 
                 previous_active_frame = frame_index;

                 break;
             }
         }

    }

   public void StartAdventure()
   {

       SwitchCharacters();

       SwitchCharacterAndName(false);
 
   }
    
    // Set Visual animation between selected character and the left other
    public void SwitchCharacters()
    {
        //Scale selected one (Later add animation for it if available)
        
        Animation selected_anim = characters_container.transform.GetChild(active_character_index).transform.GetComponent<Animation>();
        Animation deselected_anim = characters_container.transform.GetChild(1-active_character_index).transform.GetComponent<Animation>();

        // Select
        selected_anim.Play("character_selected");
        //UnSelect
        deselected_anim.Play("character_deselected");

        //Save in Progress Data
        progress_data.partners[0].index = active_character_index ;

        //Switch BD
        bd.transform.GetComponent<SpriteRenderer>().sprite = bd_frames[active_character_index];


    }

    public void SwitchCharacterAndName(bool activate_character)
    {
        //Hide character selection pan : Here we use the frame_ui to access the needed gameObject
        frames_ui[0].items[1].transform.GetChild(0).gameObject.SetActive(activate_character);
        //Show name field
        frames_ui[0].items[1].transform.GetChild(1).gameObject.SetActive(!activate_character);

        // Autofocus Next field
        m_nameInputField.Select();
        m_nameInputField.ActivateInputField();

    }

    public void SwitchNameAndCompany(bool activate_name)
    {
         string content = m_nameInputValue.text;
 
         player_name = content;

         if(content != "")
          {
                //Hide character selection pan : 
                Name_step.SetActive(activate_name);
                //Show name field
                Company_step.SetActive(!activate_name);
                // Autofocus Next field
                m_companyInputField.Select();
                m_companyInputField.ActivateInputField();
          }
         else
          {
                Name_step.transform.GetChild(1).transform.GetComponent<Animation>().Play("field_shake");
                // Autofocus Next field
                m_nameInputField.Select();
                m_nameInputField.ActivateInputField();
          } 


  
    }


 public void SaveName()
 { 
        string content = m_nameInputValue.text;
        // Saving in global progress partners
        progress_data.partners[0].name = content;        

 }

public void SaveCompany()
 { 
        string content = m_companyInputValue.text;

        PlayerPrefs.SetString("company_name" , content);

 }

 public void SavePlayerData()
 { 
        ProgressManager.Instance.SaveGlobalProgress(progress_data);
 }

 public void ActivateNavigator()
 {
    
        string content = m_companyInputValue.text;
           
        if(content != "")
        { 
            company_name = content;

            ui_navigator.SetActive(true);
            controls_activator.SetActive(true);
     
            FindObjectOfType<BDReaderEngine>().GoToBoardIndex(1);
        }
        else
        {
            Company_step.transform.GetChild(1).transform.GetComponent<Animation>().Play("field_shake");
            // Autofocus Next field
            m_companyInputField.Select();
            m_companyInputField.ActivateInputField();
        }
       
     
 }
 
 
}


[System.Serializable]
public class Frame_ui{
    public int frame_index;
    public List<GameObject> items;
}